using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MaterialUI;
using Meteor;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TrainJam.Multiplayer
{
    public class GameController : UIBehaviour
    {
        public static GameController instance { get; private set; }

        [Header("Multiplayer")]
        [SerializeField]
        [Tooltip("Check this box to try to connect to a server running at the provided URL")]
        private bool m_EnableMultiplayer;

        [SerializeField] private string m_Url = "ws://localhost:3000/websocket";

        [Header("References")] [SerializeField]
        private Sprite[] m_Sprites;

        [SerializeField] private EntityBehaviour[] m_Prefabs;

        [Header("UI Settings")] [SerializeField]
        private ScreenView m_UiScreenView;

        [SerializeField] private MaterialScreen m_ConnectingScreen;
        [SerializeField] private MaterialScreen m_LobbyScreen;
        [SerializeField] private MaterialScreen m_GameScreen;
        [SerializeField] private MaterialScreen m_TutorialScreen;
        [SerializeField] private MaterialScreen m_RoundEndScreen;

        [Header("Lobby Settings")] [SerializeField]
        private Text m_ReadyText;

        [SerializeField] private string m_ReadyTextTemplate = "Players: {0} Ready of {1} Players";
        [SerializeField] private Text m_MessageText;
        [SerializeField] private Button m_ReadyButton;
        [SerializeField] private Button m_StartGameButton;

        private readonly ReactiveProperty<MatchDocument> m_Match = new ReactiveProperty<MatchDocument>();
        private readonly StringReactiveProperty m_CurrentMatchId = new StringReactiveProperty();
        private readonly IntReactiveProperty m_CurrentPlayerId = new IntReactiveProperty(-1);
        private CompositeDisposable m_MatchDisposables = new CompositeDisposable();

        public int localPlayerId => m_CurrentPlayerId.Value;
        public IReadOnlyReactiveProperty<int> localPlayerReactiveId => m_CurrentPlayerId;
        public IReadOnlyReactiveProperty<MatchDocument> match => m_Match;

        public string matchId => m_CurrentMatchId.Value;

        public bool EnableMultiplayer => m_EnableMultiplayer;

        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }

        protected override void Start()
        {
            if (!m_EnableMultiplayer)
            {
                m_UiScreenView.Transition(m_GameScreen.screenIndex);
                return;
            }

            // Go to the connecting screen by default
            m_UiScreenView.Transition(m_ConnectingScreen.screenIndex);

            // Always show a connecting screen if you're not yet connected
            this.ObserveEveryValueChanged(ignored => Connection.Connected)
                .DistinctUntilChanged()
                .Subscribe(isConnected =>
                {
                    if (!isConnected)
                    {
                        m_UiScreenView.Transition(m_ConnectingScreen.screenIndex);
                    }
                })
                .AddTo(this);

            var connectionId = new StringReactiveProperty();

            // Keep track of my connection ID
            var connections = new Collection<ConnectionDocument>("connections");
            connections
                .Find()
                .Observe((id, ignored) => { connectionId.Value = id; })
                .AddTo(this);

            // Show the lobbies
            var lobbies = new Collection<LobbyDocument>("lobbies");
            lobbies.Find(lobby => lobby._id == "public")
                .Observe((id, doc) => { UpdateLobby(doc); },
                    (id, doc, values, keys) => UpdateLobby(doc))
                .AddTo(this);

            // Wire up to the match
            var matches = new Meteor.Collection<MatchDocument>("matches");
            matches.Find().Observe(added:
                    (id, document) =>
                    {
                        m_CurrentMatchId.Value = document._id;
                        m_CurrentPlayerId.Value = Array.IndexOf(document.players, connectionId.Value);
                        m_Match.Value = document;
                    }, changed: (id, document, values, keys) => { m_Match.Value = document; },
                    removed: (id) => { m_Match.Value = null; })
                .AddTo(this);

            // Keeps track of the instantiated entity objects
            var entities = new Collection<EntityDocument>("entities");
            var prefabs = new Dictionary<string, EntityBehaviour>();
            foreach (var kv in EntityBehaviour.prefabs)
            {
                prefabs[kv.Key] = kv.Value;
            }

            foreach (var kv in InSceneEntityPrefabs.prefabs)
            {
                prefabs[kv.Key] = kv.Value;
            }

            // First, subscribe to the entities as soon as we have a valid player id and match id
            Observable.Zip(m_CurrentPlayerId, m_CurrentMatchId,
                    (playerId, matchId) => new Tuple<int, string>(playerId, matchId))
                .Debug("Player Id and Match Id")
                .Where(tuple => tuple.Item1 != -1 && tuple.Item2 != null)
                .Subscribe(tuple =>
                {
                    entities.Find().Observe(added: (id, doc) =>
                    {
                        var instanceId = doc.sceneId ?? doc._id;
                        if (doc.sceneId == null)
                        {
                            var behaviour = prefabs[doc.prefab];
                            if (behaviour.instantiateOnAdded)
                            {
                                // Calls start which adds it to the EntityBehaviour.instances dict
                                var instance = Instantiate(behaviour);
                                instance.entity = doc;
                                instance.OnInstantiated();
                            }
                            else
                            {
                                Debug.LogError(
                                    $"GameController: {doc.prefab} should be instantiate on added since it was not found in the scene with id {id} and existing instances {string.Join(",", EntityBehaviour.instances.Keys.ToArray())}");
                                return;
                            }
                        }
                        else
                        {
                            EntityBehaviour.instances[instanceId].entity = doc;
                            EntityBehaviour.instances[instanceId].OnInstantiated();
                        }

                        var entity = EntityBehaviour.instances[instanceId];
                        entity.OnAddedInternal(doc, localPlayerId);
                    }, changed: (id, doc, changes, fields) =>
                    {
                        if (!EntityBehaviour.instances.ContainsKey(id))
                        {
                            Debug.LogError(
                                $"GameController: Changed failed because missing instance with entity ID {id} with prefab type {doc.prefab}");
                            return;
                        }

                        EntityBehaviour.instances[id].OnChangedInternal(doc, changes, fields);
                    }, removed: id =>
                    {
                        if (!EntityBehaviour.instances.ContainsKey(id))
                        {
                            Debug.LogError(
                                $"GameController: Removed failed because missing instance with entity ID {id}");
                            return;
                        }

                        var entity = EntityBehaviour.instances[id];
                        entity.OnRemovedInternal();
                        if (entity.destroyOnRemoved)
                        {
                            // Removes it from the dict too
                            Destroy(entity.gameObject);
                        }
                    }).AddTo(m_MatchDisposables);
                    StartCoroutine(MeteorEntitiesSubscription(tuple));
                })
                .AddTo(this);

            // The ready button tells the lobby (currently the public one) that we are ready to play
            m_ReadyButton.OnPointerClickAsObservable()
                .Subscribe(ignored => { StartCoroutine(MeteorReadyPlayer()); })
                .AddTo(this);

            // The start game button starts the game. Anyone can press it!
            m_StartGameButton.OnPointerClickAsObservable()
                .Subscribe(ignored => { StartCoroutine(MeteorStartGame()); })
                .AddTo(this);

            // Connect to meteor and subscribe
            StartCoroutine(MeteorStart());
        }

        private IEnumerator MeteorEntitiesSubscription(Tuple<int, string> tuple)
        {
            // This subscription takes a match id, followed by a player id
            Debug.Log(
                $"MeteorEntitiesSubscription: Subscribing to entities with matchId {tuple.Item2}, playerId {tuple.Item1}");
            var sub = Subscription.Subscribe("entities", tuple.Item2, tuple.Item1);
            sub.AddTo(m_MatchDisposables);
            yield return (Coroutine) sub;
        }

        private void UpdateLobby(LobbyDocument doc)
        {
            m_ReadyText.text = string.Format(m_ReadyTextTemplate, doc.readyCount, doc.playerCount);
            m_StartGameButton.gameObject.SetActive(doc.readyCount >= 2);
        }

        /// <summary>
        /// Performs the connection procedure, subscribes, moves to the appropriate screens
        /// </summary>
        /// <returns></returns>
        private IEnumerator MeteorStart()
        {
            yield return Connection.Connect(m_Url);

            if (!Connection.Connected)
            {
                Debug.LogError($"MeteorStart: Connection timed out");
                yield break;
            }

            // Go to the lobby screen
            m_UiScreenView.Transition(m_LobbyScreen.screenIndex);

            yield return (Coroutine) Subscription.Subscribe("data");
        }

        /// <summary>
        /// Readies the player in the public lobby
        /// </summary>
        /// <returns></returns>
        private IEnumerator MeteorReadyPlayer()
        {
            yield return (Coroutine) Method<bool>.Call("readyPlayer");
        }

        private IEnumerator MeteorStartGame()
        {
            m_MatchDisposables.Dispose();
            m_MatchDisposables = new CompositeDisposable();
            var request = Method<string>.Call("startGame", "public");
            yield return (Coroutine) request;
            if (request.Error != null)
            {
                Debug.LogError($"MeteorStartGame: {request.Error.reason}");
                yield break;
            }
        }

        public void SetBool(string entityId, int index, bool newBool)
        {
            StartCoroutine(MeteorSetBool(entityId, index, newBool));
            // TODO: Local simulation?!
        }

        private IEnumerator MeteorSetBool(string entityId, int index, bool newBool)
        {
            var request = Method<int>.Call("setBool", entityId, index, newBool);
            yield return (Coroutine) request;
        }

        public void Instantiate(int toPlayerId, string prefabToInstantiate)
        {
            StartCoroutine(MeteorInstantiate(toPlayerId, prefabToInstantiate));
        }

        private IEnumerator MeteorInstantiate(int toPlayerId, string prefabToInstantiate)
        {
            var request = Method<string>.Call("teleport", matchId, toPlayerId, prefabToInstantiate);
            yield return (Coroutine) request;
        }

        public void Deliver(string orderId)
        {
            StartCoroutine(orderId);
        }

        public IEnumerator MeteorDeliver(string orderEntityId)
        {
            var request = Method<string>.Call("delivery", orderEntityId);
            yield return (Coroutine) request;
        }
    }
}