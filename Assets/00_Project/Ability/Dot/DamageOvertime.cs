using UnityEngine;

namespace LegionKnight
{
    public partial class DamageOvertime : MonoBehaviour
    {
        [SerializeField]
        private Damageable m_Damageable;

        private float m_Timer;
        private float m_Elapsed; // Track total elapsed time
        private float m_Duration = 5f;
        private int m_DamagePerSecond = 1;

        private bool m_IsActive = false;
        [SerializeField]
        private ParticleSystem m_Effect;

        private void Update()
        {
            HandleEffect();
        }

        private void HandleEffect()
        {
            if (!IsEffectActive()) return;
            m_Elapsed += Time.deltaTime;
            m_Timer += Time.deltaTime;

            if (m_Timer >= 1f)
            {
                m_Damageable.TakeDamage(m_DamagePerSecond);
                m_Timer = 0f;
            }

            if (m_Elapsed >= m_Duration)
            {
                StopDamageOverTimeInternal();
                Debug.Log("Damage over time effect has ended.");
            }
        }

        private bool IsEffectActive()
        {
            return m_IsActive && m_Damageable != null && m_Damageable.Health > 0;
        }

        public void ApplyDamageOverTime(int damagePerSecond, float duration)
        {
            ApplyDamageOverTimeInternal(damagePerSecond, duration);
        }
        private void ApplyDamageOverTimeInternal(int damagePerSecond, float duration)
        {
            m_DamagePerSecond = damagePerSecond;
            m_Duration = duration;
            m_Timer = 0f;
            m_Elapsed = 0f; // Reset elapsed time
            m_IsActive = true;
            if (m_Effect != null)
            {
                m_Effect.Play();
            }
            else
            {
                Debug.LogWarning("No particle effect assigned for damage over time.");
            }
            Debug.Log($"Applying damage over time: {m_DamagePerSecond} damage per second for {m_Duration} seconds.");
        }

        private void StopDamageOverTimeInternal()
        {
            m_IsActive = false;
            Debug.Log("Stopping damage over time effect.");
            if (m_Effect != null)
            {
                m_Effect.Stop();
            }
            else
            {
                Debug.LogWarning("No particle effect assigned to stop.");
            }
        }

        public void StopDamageOverTime()
        {
            StopDamageOverTimeInternal();
        }
    }
}
