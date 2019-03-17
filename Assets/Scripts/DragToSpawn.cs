using System;
using TrainJam;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    /// <summary>
    /// Spawns in an item when it's dragged on the mouse plane. Shows a ghost version if specified until a radius is
    /// exceeded.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class DragToSpawn : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private float m_ScreenRadiusToSpawn;
        [SerializeField] private MovesWithMouseAlongDraggingPlane m_PreviewObject;
        [SerializeField] private Draggable m_ObjectToSpawnPrefab;
        private readonly Subject<Draggable> m_SpawnSubject = new Subject<Draggable>();

        public IObservable<Draggable> SpawnedAsObservable => m_SpawnSubject;


        protected override void Start()
        {
            // Turn off the preview object
            m_PreviewObject.gameObject.SetActive(false);
            var spawnedThisEvent = false;
            this.OnBeginDragAsObservable()
                .Subscribe(pointer =>
                {
                    spawnedThisEvent = false;
                    m_PreviewObject.gameObject.SetActive(true);
                }).AddTo(this);

            this.OnDragAsObservable()
                .Where(ignored => !spawnedThisEvent)
                .Subscribe(pointer =>
                {
                    var thisRadius = (pointer.position - pointer.pressPosition).magnitude;
                    if (thisRadius > m_ScreenRadiusToSpawn)
                    {
                        m_PreviewObject.gameObject.SetActive(false);
                        spawnedThisEvent = true;
                        var spawnedObject = Instantiate(m_ObjectToSpawnPrefab,
                            DraggingObjectPlane.instance.mousePositionOnPlane, m_PreviewObject.transform.rotation);
                        spawnedObject.gameObject.SetActive(true);
                        m_SpawnSubject.OnNext(spawnedObject);
                        spawnedObject.GetComponent<MovesWithMouseAlongDraggingPlane>().enabled = true;
                        spawnedObject.OnBeginDrag();
                    }
                }).AddTo(this);

            // Disable the preview object whenever we stop dragging.
            this.OnEndDragAsObservable()
                .Subscribe(pointer => { m_PreviewObject.gameObject.SetActive(false); })
                .AddTo(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }
    }
}