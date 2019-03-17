using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam
{
    /// <summary>
    /// As long as the script is enabled, moves the mouse along the plane.
    /// </summary>
    public sealed class MovesWithMouseAlongDraggingPlane : UIBehaviour
    {
        private Vector3 m_Velocity;
        public Vector3 Velocity => m_Velocity;
        protected override void Start()
        {
            // Find the location along the dragging object plane to place the object so it appears to be moving
            // consistently under the mouse. Use update instead of fixed update so that the item we're dragging doesn't
            // lag behind the physical touch.
            Observable.EveryUpdate()
                .Subscribe(ignored =>
                {
                    if (!enabled)
                    {
                        return;
                    }
                    
                    // Find where the pointer is on the dragging object plane, and move the object there
                    // We found the location of the raycast on the dragging object plane. Move the object there.
                    var position = DraggingObjectPlane.instance.mousePositionOnPlane;
                    m_Velocity = (position - transform.position) / Time.deltaTime;
                    transform.position = position;
                })
                .AddTo(this);
        }
    }
}