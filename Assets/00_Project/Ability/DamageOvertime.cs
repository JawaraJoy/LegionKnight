using UnityEngine;

namespace LegionKnight
{
    public class DamageOvertime : MonoBehaviour
    {
        [SerializeField]
        private Damageable m_Damageable;

        private float m_Timer;
        private float m_Duration = 5f; // Duration of the damage over time effect
        private int m_DamagePerSecond = 1; // Amount of damage to apply per second

        private bool m_IsActive = false; // Flag to check if the effect is active
        private void Update()
        {
            HandleEffect();
        }

        private void HandleEffect()
        {
            if (!IsEffectActive()) return;
            if (m_Timer < m_Duration)
            {
                m_Timer += Time.deltaTime;
                if (m_Timer >= 1f) // Apply damage every second
                {
                    m_Damageable.TakeDamage(m_DamagePerSecond);
                    m_Timer = 0f; // Reset timer after applying damage
                }
            }
            else
            {
                // Deactivate the effect after the duration
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
            m_DamagePerSecond = damagePerSecond;
            m_Duration = duration;
            m_Timer = 0f; // Reset timer
            m_IsActive = true; // Activate the effect
            Debug.Log($"Applying damage over time: {m_DamagePerSecond} damage per second for {m_Duration} seconds.");
        }
        private void StopDamageOverTimeInternal()
        {
            m_IsActive = false; // Deactivate the effect
            Debug.Log("Stopping damage over time effect.");
        }
        public void StopDamageOverTime()
        {
            StopDamageOverTimeInternal();
        }
    }
}
