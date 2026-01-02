using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class Damageable : MonoBehaviour
    {
        [SerializeField]
        private float m_MaxHealth = 100f;
        [SerializeField]
        private float m_CurrentHealth = 100f;
        [SerializeField, MMReadOnly]
        private bool m_IsAlive = true;
        [SerializeField, MMReadOnly]
        private bool m_IsTargeted = false;
        public bool IsTargeted => m_IsTargeted;
        public bool IsAlive => m_IsAlive;
        public float CurrentHealth => m_CurrentHealth;
        public float MaxHealth => m_MaxHealth;
        public float HealthPercentage => m_CurrentHealth / m_MaxHealth;
        public void SetMaxHealth(float health)
        {
            m_MaxHealth = health;
            if (m_CurrentHealth > m_MaxHealth)
            {
                m_CurrentHealth = m_MaxHealth;
            }
        }
        public void SetCurrentHealth(float health)
        {
            m_CurrentHealth = health;
            if (m_CurrentHealth <= 0f)
            {
                m_CurrentHealth = 0f;
            }
            m_IsAlive = m_CurrentHealth > 0f;
        }
        public void SetTargeted(bool targeted)
        {
            m_IsTargeted = targeted;
        }
        public void SetAlive(bool alive)
        {
            m_IsAlive = alive;
        }
    }
}
