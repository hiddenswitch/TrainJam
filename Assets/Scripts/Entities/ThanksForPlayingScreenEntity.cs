using MaterialUI;
using UnityEngine;

namespace TrainJam.Multiplayer.Entities
{
    public sealed class ThanksForPlayingScreenEntity : SceneEntityBehaviour
    {
        [SerializeField] private ScreenView m_UiScreenView;
        [SerializeField] private MaterialScreen m_ThanksForPlaying;

        protected override void OnAdded(EntityDocument entity, int localPlayerId)
        {
            m_UiScreenView.Transition(m_ThanksForPlaying.screenIndex);
        }
    }
}