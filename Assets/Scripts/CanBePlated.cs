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
    public sealed class CanBePlated : UIBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] private LayerMask m_RaycastLayers;
        [SerializeField] private float m_RaycastDepth = 10f;
        private DeliveryPlate deliveryPlate = null;

        private float progressSpeed = 0.5f;
        private float progress = 0;
        public bool alreadyFinished = false;

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
                        return;
                    }

                    if (dragging)
                    {
                    deliveryPlate = hit.collider.gameObject.GetComponent<DeliveryPlate>();
                    if (deliveryPlate == null)
                        {
                            return;
                        }
                        progress += progressSpeed * Time.fixedDeltaTime;
                        progress = Mathf.Clamp(progress, 0, 1);
                        //deliveryPlate.StartCutting(progress, alreadyFinished);
                        if (progress >= 1 && !alreadyFinished)
                        {
                            if (resultObject)
                            {
                                Instantiate(resultObject, transform.position, transform.rotation);
                                Destroy(this.gameObject);
                            }
                            alreadyFinished = true;
                            
                        }
                    }
                    /*else if (m_CuttingBoard != null)
                    {
                        StopCutting();
                    }*/
                })
                .AddTo(this);

            this.OnPointerUpAsObservable()
                .Subscribe(ignored => dragging = false)
                .AddTo(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }
    }
}