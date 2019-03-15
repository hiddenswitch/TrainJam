using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam
{
    /// <summary>
    /// Raycasts down to check if there's a <see cref="CuttingBoard"/> behaviour, and activates/deactivates it at the right time.
    /// </summary>
    public sealed class CanBeCut : UIBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] private LayerMask m_RaycastLayers;
        [SerializeField] private float m_RaycastDepth = 10f;
        private CuttingBoard m_CuttingBoard = null;

        protected override void Start()
        {
            base.Start();
            var dragging = false;
            this.OnPointerDownAsObservable()
                .Subscribe(ignored => dragging = true)
                .AddTo(this);


            // Whenever the item below this object is a cutting table, activate the cutting. Otherwise, stop it.
            Observable.EveryFixedUpdate()
                .Subscribe(ignored =>
                {
                    if (!Physics.Raycast(transform.position, Vector3.down, out var hit, m_RaycastDepth,
                        m_RaycastLayers.value))
                    {
                        StopCutting();
                        return;
                    }

                    if (dragging)
                    {
                        m_CuttingBoard = hit.collider.gameObject.GetComponent<CuttingBoard>();
                        if (m_CuttingBoard == null)
                        {
                            return;
                        }
                        m_CuttingBoard.StartCutting();
                    }
                    else if (m_CuttingBoard != null)
                    {
                        StopCutting();
                    }
                })
                .AddTo(this);

            this.OnPointerUpAsObservable()
                .Subscribe(ignored => dragging = false)
                .AddTo(this);
        }

        private void StopCutting()
        {
            if (m_CuttingBoard == null)
            {
                return;
            }

            m_CuttingBoard.StopCutting();
            m_CuttingBoard = null;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }
    }
}