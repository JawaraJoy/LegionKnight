using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class LaserGun : MonoBehaviour
    {
        [SerializeField]
        private float m_Distance = 100f;
        [SerializeField]
        private Transform m_FirePoint;
        [SerializeField]
        private LineRenderer m_Renderer;
        [SerializeField]
        private LayerMask m_HitMask;
        [SerializeField]
        private float m_FireDuration = 2f;
        [SerializeField]
        private Vector2 m_FireDirection = Vector2.right;
        [SerializeField]
        private float m_BeamGrowSpeed = 200f;
        [SerializeField]
        private float m_BeamShrinkSpeed = 300f;
        [SerializeField, Range(0.05f, 1f)]
        private float m_ShrinkRatio = 0.2f;
        [SerializeField]
        private float m_InitialWidth = 0.2f;

        [Header("Shrink Options")]
        [SerializeField]
        private bool m_ShrinkLength = true;
        [SerializeField]
        private bool m_ShrinkWidth = true;

        [Header("Damage Options")]
        [SerializeField]
        private float m_DamageInterval = 0.5f; // Delay between damage ticks in seconds

        private float m_FireTimer = 0f;
        private bool m_IsFiring = false;
        private float m_CurrentBeamLength = 0f;
        private float m_CurrentWidth = 0f;
        private Vector3 m_EndPoint;
        private float m_DamageTimer = 0f;

        [SerializeField]
        private int m_Damage = 1;

        [SerializeField]
        private UnityEvent m_OnStartFiring;

        [SerializeField]
        private UnityEvent m_OnStopFiring;

        public void SetDamage(int damage)
        {
            m_Damage = damage;
        }

        private void Awake()
        {
            if (m_Renderer != null)
            {
                m_Renderer.positionCount = 2;
                m_Renderer.startWidth = m_InitialWidth;
                m_Renderer.endWidth = m_InitialWidth;
            }
        }

        public void StartFiring()
        {
            m_IsFiring = true;
            m_FireTimer = 0f;
            m_CurrentBeamLength = 0f;
            m_CurrentWidth = m_InitialWidth;
            m_DamageTimer = 0f;
            if (m_Renderer != null)
            {
                m_Renderer.enabled = true;
                m_Renderer.startWidth = m_InitialWidth;
                m_Renderer.endWidth = m_InitialWidth;
            }
            m_OnStartFiring?.Invoke();
        }

        public void StopFiring()
        {
            m_IsFiring = false;
            if (m_Renderer != null)
                m_Renderer.enabled = false;
            m_OnStopFiring?.Invoke();
        }

        private void Update()
        {
            if (m_IsFiring)
            {
                TimeFiring();
            }
        }

        private void TimeFiring()
        {
            m_FireTimer += Time.deltaTime;
            m_DamageTimer += Time.deltaTime;
            float maxLength = m_Distance;
            float shrinkStart = m_FireDuration * (1f - m_ShrinkRatio);

            if (m_FireTimer < shrinkStart)
            {
                // Grow phase
                m_CurrentBeamLength = Mathf.Min(m_BeamGrowSpeed * m_FireTimer, maxLength);
                m_CurrentWidth = m_InitialWidth;
            }
            else
            {
                // Shrink phase
                if (m_ShrinkLength)
                    m_CurrentBeamLength = Mathf.Max(m_CurrentBeamLength - m_BeamShrinkSpeed * Time.deltaTime, 0f);
                if (m_ShrinkWidth)
                    m_CurrentWidth = Mathf.Max(m_CurrentWidth - m_BeamShrinkSpeed * Time.deltaTime * (m_InitialWidth / maxLength), 0f);
            }

            Vector2 origin = m_FirePoint.position;
            Vector2 direction = m_FireDirection.normalized;
            float rayLength = Mathf.Max(m_CurrentBeamLength, 0f);

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayLength, m_HitMask);

            m_EndPoint = origin + (direction * rayLength);
            if (hit.collider != null)
            {
                m_EndPoint = hit.point;
                if (m_DamageTimer >= m_DamageInterval)
                {
                    if (hit.collider.TryGetComponent<Damageable>(out var damageable))
                    {
                        damageable.TakeDamage(m_Damage);
                    }
                    m_DamageTimer = 0f;
                }
            }

            if (m_Renderer != null)
            {
                m_Renderer.enabled = true;
                m_Renderer.SetPosition(0, origin);
                m_Renderer.SetPosition(1, m_EndPoint);
                m_Renderer.startWidth = m_CurrentWidth;
                m_Renderer.endWidth = m_CurrentWidth;
            }

            if (m_FireTimer >= m_FireDuration || (m_CurrentBeamLength <= 0f && m_CurrentWidth <= 0f))
            {
                StopFiring();
            }
        }
    }
}