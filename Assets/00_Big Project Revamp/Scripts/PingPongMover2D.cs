using UnityEngine;

namespace Rush
{
    public class PingPongMover2D : MonoBehaviour, IUpdater
    {
        [Header("Movement Settings")]
        [SerializeField] private float m_Distance = 2f;
        [SerializeField] private float m_Speed = 2f;
        [SerializeField] private Vector2 m_Direction = Vector2.right;
        [SerializeField] private float m_RandomSpawnRadius = 2f;
        [SerializeField] private bool m_RandomizeOnEnable = true;
        [SerializeField] private bool m_KeepZ = true; // untuk 2D (biar ga geser depth)
        [SerializeField] private Camera m_TargetCamera;
        [SerializeField] private bool m_ClampInsideCamera = true;
        [SerializeField] private float m_EdgePadding = 0.5f;

        [Header("Optional")]
        [SerializeField] private bool m_UseLocalSpace = false;

        private Vector3 m_StartPosition;
        private Vector3 m_NormalizedDirection;

        public bool IsActive => gameObject.activeInHierarchy;

        private void OnEnable()
        {
            Vector3 basePosition = m_UseLocalSpace ? transform.localPosition : transform.position;

            if (m_RandomizeOnEnable)
            {
                Vector2 randomOffset2D = Random.insideUnitCircle * m_RandomSpawnRadius;
                Vector3 randomOffset = new Vector3(randomOffset2D.x, randomOffset2D.y, 0f);

                if (m_KeepZ)
                {
                    randomOffset.z = 0f;
                }

                basePosition += randomOffset;

                if (m_UseLocalSpace)
                    transform.localPosition = basePosition;
                else
                    transform.position = basePosition;
            }

            m_StartPosition = basePosition;

            // Normalize sekali saja
            m_NormalizedDirection = m_Direction.normalized;

            ClampMovementInsideCamera();

            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }
        public void Tick()
        {
            float pingPong = Mathf.PingPong(Time.time * m_Speed, m_Distance);

            Vector3 offset = m_NormalizedDirection * pingPong;

            if (m_UseLocalSpace)
            {
                transform.localPosition = m_StartPosition + offset;
            }
            else
            {
                transform.position = m_StartPosition + offset;
            }
        }

        private Bounds GetCameraBounds()
        {
            Camera cam = m_TargetCamera != null ? m_TargetCamera : Camera.main;

            float height = cam.orthographicSize * 2f;
            float width = height * cam.aspect;

            Vector3 center = cam.transform.position;

            return new Bounds(center, new Vector3(width, height, 0f));
        }
        private void ClampMovementInsideCamera()
        {
            if (!m_ClampInsideCamera) return;

            Bounds camBounds = GetCameraBounds();

            Vector3 dir = m_NormalizedDirection;

            // Hitung max distance ke tiap sisi
            float maxDistance = m_Distance;

            if (Mathf.Abs(dir.x) > 0.001f)
            {
                float limitX = dir.x > 0
                    ? camBounds.max.x - m_StartPosition.x - m_EdgePadding
                    : m_StartPosition.x - camBounds.min.x - m_EdgePadding;

                maxDistance = Mathf.Min(maxDistance, Mathf.Abs(limitX));
            }

            if (Mathf.Abs(dir.y) > 0.001f)
            {
                float limitY = dir.y > 0
                    ? camBounds.max.y - m_StartPosition.y - m_EdgePadding
                    : m_StartPosition.y - camBounds.min.y - m_EdgePadding;

                maxDistance = Mathf.Min(maxDistance, Mathf.Abs(limitY));
            }

            m_Distance = Mathf.Max(0f, maxDistance);
        }
    }
}