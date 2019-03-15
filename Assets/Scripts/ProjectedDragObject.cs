using System.Collections.Generic;
using DG.Tweening;
using MonsterMatch;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam
{
    public class ProjectedDragObject : UIBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] private Rigidbody m_Rigidbody;
        Sequence squishSequence;

        protected void Start()
        {
            //squishSequence = DOTween.Sequence();
            //squishSequence.Insert(0f, transform.DOScale(transform.localScale.x*0.9f, 0.05f));
            var dragging = false;
            var velocity = Vector3.zero;
            // Immediately start dragging as soon as the pointer goes down. This behaves a little differently than a
            // proper drag, which is less responsive (it requires a delay to elapse).
            this.OnPointerDownAsObservable()
                .Subscribe(pointer =>
                {
                    //squishSequence.Play();
                    dragging = true;
                    velocity = Vector3.zero;
                    m_Rigidbody.isKinematic = true;
                })
                .AddTo(this);

            // Find the location along the dragging object plane to place the object so it appears to be moving
            // consistently under the mouse. Use update instead of fixed update so that the item we're dragging doesn't
            // lag behind the physical touch.
            Observable.EveryUpdate()
                .Subscribe(ignored =>
                {
                    if (!dragging)
                    {
                        return;
                    }

                    // Find where the pointer is on the dragging object plane, and move the object there
                    // We found the location of the raycast on the dragging object plane. Move the object there.
                    var position = DraggingObjectPlane.instance.mousePositionOnPlane;
                    velocity = (position - transform.position) / Time.deltaTime;
                    transform.position = position;
                })
                .AddTo(this);

            this.OnPointerUpAsObservable()
                .Subscribe(pointer =>
                {
                    dragging = false;
                    // If there are no other active animations on this object, disable its kinematics
                    if (DOTween.IsTweening(transform))
                    {
                        return;
                    }

                    m_Rigidbody.isKinematic = false;
                    m_Rigidbody.velocity = velocity;
                })
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