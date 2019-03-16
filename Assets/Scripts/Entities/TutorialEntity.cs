using MaterialUI;
using UnityEngine;

namespace TrainJam.Multiplayer.Entities
{
    public sealed class TutorialEntity : TimeLeftEntity
    {
        [SerializeField] private ScreenView m_UiScreenView;
        [SerializeField] private MaterialScreen m_GameScreen;
        [SerializeField] private MaterialScreen m_TutorialScreen;

        protected override void OnAdded(EntityDocument entity, int localPlayerId)
        {
            base.OnAdded(entity, localPlayerId);
            m_UiScreenView.Transition(m_TutorialScreen.screenIndex);
        }

        protected override void OnElapsed()
        {
            base.OnElapsed();
            m_UiScreenView.Transition(m_GameScreen.screenIndex);
        }
    }
}