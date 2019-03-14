using System.Collections;
using System.Collections.Generic;
using MaterialUI;
using Meteor;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TrainJam.Multiplayer
{
    public class GameController : UIBehaviour
    {
        [Header("Multiplayer")]
        [SerializeField]
        [Tooltip("Check this box to try to connect to a server running at the provided URL")]
        private bool m_EnableMultiplayer;

        [SerializeField] private string m_Url = "ws://localhost:3000/websocket";


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

        private StringReactiveProperty m_CurrentMatchId = new StringReactiveProperty();

        public IReadOnlyReactiveProperty<string> CurrentMatchId => m_CurrentMatchId;

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

            // Show the lobbies
            var lobbies = new Collection<LobbyDocument>("lobbies");
            lobbies.Find(lobby => lobby._id == "public")
                .Observe((id, doc) => { UpdateLobby(doc); },
                    (id, doc, values, keys) => UpdateLobby(doc));

            // Wire up to the match
            var matches = new Meteor.Collection<MatchDocument>("matches");

            m_ReadyButton.OnPointerClickAsObservable()
                .Subscribe(ignored => { StartCoroutine(MeteorReadyPlayer()); })
                .AddTo(this);

            m_StartGameButton.OnPointerClickAsObservable()
                .Subscribe(ignored => { StartCoroutine(MeteorStartGame()); })
                .AddTo(this);

            // Whenever a match is set, go to the game screen for now.
            m_CurrentMatchId.Where(id => !string.IsNullOrEmpty(id))
                .Subscribe(matchId => m_UiScreenView.Transition(m_GameScreen.screenIndex));

            // Connect to meteor and subscribe
            StartCoroutine(MeteorStart());
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

            yield return (Coroutine) Subscription.Subscribe("lobbies");
            yield return (Coroutine) Subscription.Subscribe("matches");
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
            var request = Method<string>.Call("startGame", "public");
            yield return (Coroutine) request;
            if (request.Error != null)
            {
                Debug.LogError($"MeteorStartGame: {request.Error.reason}");
                yield break;
            }

            // Get the match ID out of here
            m_CurrentMatchId.Value = request.Response;
        }
    }

    public sealed class MatchDocument : MongoDocument
    {
        public List<string> players;
        public string level;
        public int round;
    }

    public sealed class LobbyDocument : MongoDocument
    {
        public List<ConnectionDocument> connections;
        public int playerCount;
        public int readyCount;
        public string message;
    }

    public sealed class ConnectionDocument
    {
        public string id;
        public bool ready;
    }
}