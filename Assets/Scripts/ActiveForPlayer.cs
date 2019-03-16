using TrainJam.Multiplayer;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam
{
    public sealed class ActiveForPlayer : UIBehaviour
    {
        [SerializeField] private int m_ActiveForPlayerId;

        protected override void Start()
        {
            base.Start();
            GameController.instance.localPlayerReactiveId
                .Subscribe(playerId => { gameObject.SetActive(playerId == m_ActiveForPlayerId); })
                .AddTo(this);
            gameObject.SetActive(false);
        }
    }
}