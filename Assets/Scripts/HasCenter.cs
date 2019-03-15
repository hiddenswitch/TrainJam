using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam
{
    [RequireComponent(typeof(Collider))]
    public sealed class HasCenter : UIBehaviour
    {
        [SerializeField] private Transform m_CenterPosition;

        public Transform centerPosition => m_CenterPosition;

        protected override void Awake()
        {
            base.Awake();
            m_CenterPosition = m_CenterPosition ? m_CenterPosition : transform;
        }
    }
}