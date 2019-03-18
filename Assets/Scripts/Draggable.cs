using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam
{
    /// <summary>
    /// Indicates a item that can be dragged and dropped, moving along the mouse plane and throwing off its physics.
    /// </summary>
    [RequireComponent(typeof(MovesWithMouseAlongDraggingPlane))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class Draggable : UIBehaviour, IPointerUpHandler, IPointerDownHandler, IDropHandler
    {
        private readonly Subject<PointerEventData> m_SimulatedDrop = new Subject<PointerEventData>();
        private Rigidbody m_Rigidbody;
        private MovesWithMouseAlongDraggingPlane m_Mover;

        protected override void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Mover = GetComponent<MovesWithMouseAlongDraggingPlane>();

            // Immediately start dragging as soon as the pointer goes down. This behaves a little differently than a
            // proper drag, which is less responsive (it requires a delay to elapse).
            this.OnPointerDownAsObservable()
                .Subscribe(pointer => { OnBeginDrag(); })
                .AddTo(this);

            // We'll wait a frame before actually checking if any tweens have been scheduled
            Observable.Merge(
                    this.OnDropAsObservable(),
                    this.OnPointerUpAsObservable(),
                    this.OnEndDragAsObservable())
                .Subscribe(pointer => { OnEndDrag(); }).AddTo(this);
        }

        internal void OnBeginDrag()
        {
            // Cancel tweening actions on the transform if we're permitted to
            if (DOTween.IsTweening(transform) && !GetComponent<TweenCannotBeCancelled>())
            {
                DOTween.Kill(transform, false);
            }

            m_Mover.enabled = true;
            m_Rigidbody.isKinematic = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnDrop(PointerEventData eventData)
        {
        }

        internal void OnEndDrag()
        {
            // If there are no other active animations on this object, reenable its kinematics
            if (DOTween.IsTweening(transform))
            {
                return;
            }

            m_Mover.enabled = false;
            m_Rigidbody.isKinematic = false;
            m_Rigidbody.velocity = m_Mover.Velocity;
        }
    }
}