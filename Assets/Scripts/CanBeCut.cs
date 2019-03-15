using DG.Tweening;
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

        private float progressSpeed = 0.5f;
        private float progress = 0;
        public bool notCuttableOrFinished = false;

        public GameObject resultObject;

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
                        progress += progressSpeed * Time.fixedDeltaTime;
                        progress = Mathf.Clamp(progress, 0, 1);
                        m_CuttingBoard.StartCutting(progress, notCuttableOrFinished);
                        if (progress >= 1 && !notCuttableOrFinished)
                        {
                            if (resultObject)
                            {
                                Instantiate(resultObject, transform.position, transform.rotation);
                                Destroy(this.gameObject);
                            }
                            notCuttableOrFinished = true;
                            
                        }
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