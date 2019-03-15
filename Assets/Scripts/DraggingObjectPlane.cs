using MonsterMatch;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam
{
    public class DraggingObjectPlane : UIBehaviour, IDragHandler
    {
        [SerializeField] private Collider m_Collider;
        [SerializeField] Camera m_Camera;
        private Vector3 m_LastPosition;
        private RaycastHit[] m_Results = new RaycastHit[10];
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

        public Vector3 mousePositionOnPlane => m_LastPosition;

        private void Update()
        {
            var pointer = ContinuousStandaloneInputModule.instance.pointerEventData;
            if (pointer == null)
            {
                return;
            }

            var ray = m_Camera.ScreenPointToRay(pointer.position);
            var size = Physics.RaycastNonAlloc(ray, m_Results, 100f);
            foreach (var hit in m_Results)
            {
                if (hit.collider?.gameObject == gameObject)
                {
                    m_LastPosition = hit.point;
                    return;
                }
            }
        }
    }
}