using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam
{
    public class DraggingObjectPlane : UIBehaviour, IDragHandler
    {
        [SerializeField] private Collider m_Collider;
        public static DraggingObjectPlane instance { get; private set; }

        public Collider collider => m_Collider;

        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }

        public void OnDrag(PointerEventData eventData)
        {
        }
    }
}