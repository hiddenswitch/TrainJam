using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam
{
    [RequireComponent(typeof(Collider))]
    public sealed class CanBeCentered : UIBehaviour, IPointerDownHandler
    {
        [SerializeField] private float m_CenteringDuration = 0.5f;

        protected override void Start()
        {
            base.Start();

            Sequence sequence = null;
            var used = false;
            // When we're in an object that has a centering position, move towards the center position.
            this.OnTriggerEnterAsObservable()
                .Subscribe(col =>
                {
                    // Mark the trigger as used until we refresh it by clicking on it.
                    if (used)
                    {
                        return;
                    }
                    used = true;
                    // Check if we're triggering a trash
                    var center = col.gameObject.GetComponent<HasCenter>();
                    if (!center)
                    {
                        return;
                    }

                    // Set me to kinematic just in case
                    var rigidbody = GetComponent<Rigidbody>();
                    if (rigidbody)
                    {
                        rigidbody.isKinematic = true;
                    }

                    sequence?.Kill();
                    sequence = DOTween.Sequence();
                    sequence.Insert(0f, transform.DOMove(center.centerPosition.position, m_CenteringDuration));
                    sequence.SetEase(Ease.InQuad);
                    sequence.Play();
                })
                .AddTo(this);

            // Interrupt centering when we click on the object
            this.OnPointerDownAsObservable()
                .Subscribe(ignored =>
                {
                    used = false;
                    if (sequence?.IsActive() ?? false)
                    {
                        sequence.Kill();
                        sequence = null;
                    }
                })
                .AddTo(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }
    }
}