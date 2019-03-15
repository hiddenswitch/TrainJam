using MaterialUI;
using UnityEngine;

namespace TrainJam.Multiplayer.Entities
{
    public sealed class EndGameScreenEntity : SceneEntityBehaviour
    {
        [SerializeField] private ScreenView m_UiScreenView;
        [SerializeField] private MaterialScreen m_GameWonScreen;
        [SerializeField] private MaterialScreen m_GameLostScreen;
        public bool isGameOver => entity?.bools[0] ?? false;
        public bool isGameWon => entity?.bools[1] ?? false;

        protected override void OnBooleansSet(bool[] newValues)
        {
            if (isGameOver)
            {
                m_UiScreenView.Transition(isGameWon ? m_GameWonScreen.screenIndex : m_GameLostScreen.screenIndex);
            }
        }
    }
}