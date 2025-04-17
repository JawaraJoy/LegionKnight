using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class Damageable : Contact2D
    {
        [SerializeField]
        private int m_Damage;
        [SerializeField]
        private int m_Health;
        [SerializeField]
        private int m_Shield;
        [SerializeField]
        private int m_Barrier;
        private int m_CurrentHealth;
        [SerializeField]
        private UnityEvent m_OnDeath = new();
        [SerializeField]
        private UnityEvent<float> m_OnHealthRateChanged = new();
        [SerializeField]
        private UnityEvent<int> m_OnShieldChanged = new();
        [SerializeField]
        private UnityEvent<int> m_OnBarrierChanged = new();
        [SerializeField]
        private UnityEvent<int> m_OnHealthChanged = new();
        [SerializeField]
        private UnityEvent m_OnProtectGone = new();
        private void OnEnable()
        {
            m_CurrentHealth = m_Health;
        }
        protected override void OnContactedBehaviourInvoke(GameObject other)
        {
            base.OnContactedBehaviourInvoke(other);

            if (other.TryGetComponent(out Damageable projectile))
            {
                if (!IsProtectGoneInternal())
                {
                    Destroy(other);
                }
                TakeDamageInternal(projectile.Damage);
                //Destroy(projectile.gameObject);
            }
        }
        private bool IsProtectGoneInternal()
        {
            return m_Shield < 1 && m_Barrier < 1;
        }
        public int Damage => m_Damage;
        public int Health => m_Health;
        public int Shield => m_Shield;
        public int CurrentHealth => m_CurrentHealth;
        private float GetHealthRateInternal()
        {
            return (float)m_CurrentHealth / (float)m_Health;
        }
        private void ClampHealth()
        {
            m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, m_Health);
            OnHealthRateChangedInvoke(GetHealthRateInternal());
        }
        private void OnHealthRateChangedInvoke(float rate)
        {
            m_OnHealthRateChanged?.Invoke(rate);
        }
        private void OnShieldChangedInvoke(int shield)
        {
            m_OnShieldChanged?.Invoke(shield);
        }
        private void OnHealthChangedInvoke(int health)
        {
            m_OnHealthChanged?.Invoke(health);
        }
        private void OnBarrierChangedInvoke(int barrier)
        {
            m_OnBarrierChanged?.Invoke(barrier);
        }
        private void OnProtectGoneInvoke()
        {
            m_OnProtectGone?.Invoke();
            Debug.Log($"Protect Gone"); 
        }
        public void Init(int damage, int health)
        {
            m_Damage = damage;
            m_Health = health;
            m_CurrentHealth = m_Health;
        }
        public void TakeDamage(int damage)
        {
            TakeDamageInternal(damage);
        }
        protected void TakeDamageInternal(int damage)
        {
            if (m_Barrier > 0 || m_Shield > 0)
            {
                if (m_Barrier > 0)
                {
                    m_Barrier--;
                    OnBarrierChangedInvoke(m_Barrier);
                    if (m_Barrier < 0)
                    {
                        m_Barrier = 0;
                    }
                }
                if (m_Shield > 0)
                {
                    m_Shield -= damage;
                    OnShieldChangedInvoke(m_Shield);
                    if (m_Shield < 0)
                    {
                        m_Shield = 0;
                    }
                }
            }
            else
            {
                m_CurrentHealth -= damage;
                OnHealthChangedInvoke(m_CurrentHealth);
                ClampHealth();
            }
            if (IsProtectGoneInternal())
            {
                OnProtectGoneInvoke();
            }
            DeathHandler();
        }
        public void AddHealth(int health)
        {
            m_Health += health;
            m_CurrentHealth += health;
            ClampHealth();
        }
        public void AddShield(int shield)
        {
            m_Shield += shield;
        }
        public void AddBarrier(int barrier)
        {
            m_Barrier += barrier;
        }
        public void SetShield(int shield)
        {
            m_Shield = shield;
        }
        public void SetBarrier(int barrier)
        {
            m_Barrier = barrier;
        }
        protected void HealInternal(int heal)
        {
            m_CurrentHealth += heal;
            ClampHealth();
        }

        private void DeathHandler()
        {
            if (m_CurrentHealth < 1)
            {
                OnDeathInvoke();
            }
        }
        protected virtual void OnDeathInvoke()
        {
            m_OnDeath?.Invoke();
        }
    }
}
