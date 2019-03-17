using TrainJam.Multiplayer;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam
{
    public sealed class StagePlayerController : UIBehaviour
    {
        [SerializeField] private StagePlayersPlayerIdTuple m_Configuration;
        [SerializeField] private GameObject[] m_Objects = new GameObject[0];

        protected override void Start()
        {
            base.Start();
            GameController.instance.match
                .Where(ignored => GameController.instance.EnableMultiplayer)
                .Select(match => new StagePlayersPlayerIdTuple()
                {
                    ActiveForPlayerId = GameController.instance.localPlayerId,
                    ActiveForStage = match.stage,
                    NumberOfPlayers = match.playerCount
                })
                .DistinctUntilChanged()
                .Subscribe(configuration => { Set(configuration == m_Configuration); })
                .AddTo(this);
            Set(false);
        }

        private void Set(bool thisStage)
        {
            if (m_Objects.Length == 0)
            {
                gameObject.SetActive(thisStage);
            }
            else
            {
                foreach (var controlledObject in m_Objects)
                {
                    controlledObject.SetActive(thisStage);
                }
            }
        }
    }
}