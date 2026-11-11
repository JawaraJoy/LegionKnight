using UnityEngine;

namespace Rush
{
    public class PingPongMover2D : MonoBehaviour, IUpdater
    {
        [Header("Movement Settings")]
        [SerializeField] private float m_Distance = 2f;
        [SerializeField] private float m_Speed = 2f;
        [SerializeField] private Vector2 m_Direction = Vector2.right;

        [Header("Phase Settings (IMPORTANT)")]
        [SerializeField] private bool m_RandomizePhase = true;
        [SerializeField] private float m_PhaseOffset;

        [Header("Spawn Randomization (Optional)")]
        [SerializeField] private bool m_RandomizeOnEnable = false;
        [SerializeField] private float m_RandomSpawnRadius = 2f;

        [Header("Optional")]
        [SerializeField] private bool m_UseLocalSpace = false;
        [SerializeField] private bool m_KeepZ = true;

        private bool m_HasAssignedSpawnPosition;
        private Vector3 m_StartPosition;
        private Vector3 m_PreviousOffset;

        public void PrepareForSpawn()
        {
            PrepareForSpawnInternal();
        }

        private void PrepareForSpawnInternal()
        {
            m_HasAssignedSpawnPosition = true;
        }
        private Vector3 m_NormalizedDirection;
        private float m_InitialOffset;
        private float CurrentOffset => Mathf.PingPong((Time.time + m_PhaseOffset) * m_Speed, m_Distance);

        public bool IsActive => gameObject.activeInHierarchy;

        // =========================
        // UNITY LIFECYCLE
        // =========================
        private void OnEnable()
        {
            Vector3 basePosition = m_UseLocalSpace ? transform.localPosition : transform.position;

            // Optional random reposition
            if (m_RandomizeOnEnable && !m_HasAssignedSpawnPosition)
            {
                Vector2 offset2D = Random.insideUnitCircle * m_RandomSpawnRadius;
                Vector3 offset = new Vector3(offset2D.x, offset2D.y, 0f);

                if (m_KeepZ)
                    offset.z = 0f;

                basePosition += offset;

                if (m_UseLocalSpace)
                    transform.localPosition = basePosition;
                else
                    transform.position = basePosition;
            }

            m_HasAssignedSpawnPosition = false;
            m_StartPosition = basePosition;
            m_PreviousOffset = Vector3.zero;

            // Normalize once
            m_NormalizedDirection = m_Direction.normalized;

            // 🔥 KEY FIX: random phase biar tidak sync
            if (m_RandomizePhase)
            {
                m_PhaseOffset = Random.Range(0f, 100f);
            }

            m_InitialOffset = CurrentOffset;
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }

private void OnDisable()
        {
            UpdateBank updateBank = UpdateBank.Instance;
            if (updateBank != null)
            {
                updateBank.UnregisterUpdateTick(gameObject);
            }
        }


        // =========================
        // UPDATE LOOP
        // =========================
        public void Tick()
        {
            Vector3 offset = m_NormalizedDirection * (CurrentOffset - m_InitialOffset);
            Vector3 current = m_UseLocalSpace ? transform.localPosition : transform.position;
            // Preserve external displacement (pull, knockback, teleport) in the patrol anchor.
            m_StartPosition += current - (m_StartPosition + m_PreviousOffset);

            if (m_UseLocalSpace)
            {
                transform.localPosition = m_StartPosition + offset;
            }
            else
            {
                transform.position = m_StartPosition + offset;
            }
            m_PreviousOffset = offset;
        }

        // =========================
        // DEBUG
        // =========================
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(
                m_UseLocalSpace ? transform.parent?.position ?? transform.position : transform.position,
                m_RandomSpawnRadius
            );
        }
    }
}