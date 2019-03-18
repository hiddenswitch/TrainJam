using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TrainJam
{
    /// <summary>
    /// Places walls around the frustrum of the camera. Each collider should have its own transform in order for the
    /// transformation to work correctly.
    /// </summary>
    public sealed class PlaceWallsForCamera : UIBehaviour
    {
        [SerializeField] private LayerMask m_DraggingPlaneLayers;
        [SerializeField] private Camera m_Camera;
        [SerializeField] BoxCollider[] m_WallColliders = new BoxCollider[0];

        private void SetDirty()
        {
            if (m_WallColliders.Length < 4)
            {
                Debug.LogError($"PlaceWallsForCamera: Requires 4 wall colliders, got {m_WallColliders.Length}");
                return;
            }

            if (m_Camera == null)
            {
                Debug.LogError($"PlaceWallsForCamera: Requires camera");
                return;
            }

            // Raycast to depth plane
            var corners = new[]
            {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 0),
                new Vector3(0, 1, 0),
            };

            if (!Physics.Raycast(m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)),
                out var centerHit,
                m_Camera.farClipPlane,
                m_DraggingPlaneLayers.value))
            {
                Debug.LogError($"PlaceWallsForCamera: Failed to find center, check your collision layer");
                return;
            }

            for (var i = 0; i < 4; i++)
            {
                var firstCorner = corners[i % 4];
                var secondCorner = corners[(i + 1) % 4];
                var cameraSpacePosition = (firstCorner + secondCorner) / 2;

                var wallCollider = m_WallColliders[i];

                foreach (var collider in new[] {wallCollider})
                {
                    collider.transform.position = Vector3.zero;
                    collider.transform.rotation = Quaternion.identity;
                }

                var ray = m_Camera.ViewportPointToRay(cameraSpacePosition);
                if (!Physics.Raycast(ray, out var cameraSpacePositionHit, m_Camera.farClipPlane,
                    m_DraggingPlaneLayers.value))
                {
                    Debug.LogError(
                        $"PlaceWallsForCamera: Failed to find camera space position for wall {i}, check your collision layer");
                    return;
                }

                var worldSpacePosition = cameraSpacePositionHit.point;
                ray = m_Camera.ViewportPointToRay(firstCorner);
                if (!Physics.Raycast(ray, out var firstCornerHit, m_Camera.farClipPlane, m_DraggingPlaneLayers.value))
                {
                    Debug.LogError(
                        $"PlaceWallsForCamera: Failed to find first corner position for wall {i}, check your collision layer");
                    return;
                }

                ray = m_Camera.ViewportPointToRay(secondCorner);
                if (!Physics.Raycast(ray, out var secondCornerHit, m_Camera.farClipPlane, m_DraggingPlaneLayers.value))
                {
                    Debug.LogError(
                        $"PlaceWallsForCamera: Failed to find second corner position for wall {i}, check your collision layer");
                    return;
                }

                var scaleVector = firstCornerHit.point -
                                  secondCornerHit.point;
                var width = 1f;
                for (var j = 0; j < 3; j++)
                {
                    width = Mathf.Max(width, Mathf.Abs(scaleVector[j]));
                }

                width *= 2f;
                const float height = 100f;
                const float zScale = 10f;
                var towardsCenter = centerHit.point - worldSpacePosition;

                wallCollider.transform.position = worldSpacePosition - 0.5f * zScale * towardsCenter.normalized;
                wallCollider.transform.forward = towardsCenter;
                wallCollider.transform.localScale = new Vector3(width, height, zScale);
            }
        }

        protected override void Awake()
        {
            SetDirty();

            Observable.EveryEndOfFrame()
                .Select(ignored => Screen.currentResolution)
                .DistinctUntilChanged()
                .Subscribe(ignored => SetDirty())
                .AddTo(this);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            SetDirty();
        }
#endif
    }
}