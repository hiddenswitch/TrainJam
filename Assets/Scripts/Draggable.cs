using DG.Tweening;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

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
        protected override void Start()
        {
            var rigidbody = GetComponent<Rigidbody>();
            var mover = GetComponent<MovesWithMouseAlongDraggingPlane>();
            var velocityOnPointerUp = Vector3.zero;

            // Immediately start dragging as soon as the pointer goes down. This behaves a little differently than a
            // proper drag, which is less responsive (it requires a delay to elapse).
            this.OnPointerDownAsObservable()
                .Subscribe(pointer => { OnBeginDrag(mover, rigidbody); })
                .AddTo(this);

            this.OnPointerUpAsObservable()
                .Subscribe(pointer => { velocityOnPointerUp = mover.Velocity; })
                .AddTo(this);

            // We'll wait a frame before actually checking if any tweens have been scheduled
            this.OnDropAsObservable()
                .Merge(this.OnPointerUpAsObservable())
                .DelayFrame(1)
                .Debug("Received pointer up")
                .Subscribe(pointer =>
                {
                    // If there are no other active animations on this object, reenable its kinematics
                    if (DOTween.IsTweening(transform))
                    {
                        return;
                    }

                    mover.enabled = false;
                    rigidbody.isKinematic = false;
                    rigidbody.velocity = velocityOnPointerUp;
                }).AddTo(this);
        }

        internal void OnBeginDrag(MovesWithMouseAlongDraggingPlane mover = null, Rigidbody rigidbody = null)
        {
            mover = mover ? mover : GetComponent<MovesWithMouseAlongDraggingPlane>();
            rigidbody = rigidbody ? rigidbody : GetComponent<Rigidbody>();
            // Cancel tweening actions on the transform if we're permitted to
            if (DOTween.IsTweening(transform) && !GetComponent<TweenCannotBeCancelled>())
            {
                DOTween.Kill(transform, false);
            }

            mover.enabled = true;
            rigidbody.isKinematic = true;
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
    }
}