using MoreMountains.Tools;
using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class Damageable : Contact2D
    {
        [SerializeField]
        protected int m_Damage;
        [SerializeField]
        protected int m_Health;
        [SerializeField]
        private int m_Shield;
        [SerializeField]
        private int m_Barrier;
        protected int m_CurrentHealth;
        [SerializeField]
        private UnityEvent m_OnDeath = new();
        [SerializeField]
        private UnityEvent<int> m_OnDamageTaken = new();
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
                TakeDamageInternal(projectile.Damage);
                //Destroy(projectile.gameObject);
            }
        }
        private bool IsProtectGoneInternal()
        {
            return IsShieldGoneInternal() && IsBarrierGoneInternal();
        }
        private bool IsShieldGoneInternal()
        {
            return m_Shield < 1;
        }
        private bool IsBarrierGoneInternal()
        {
            return m_Barrier < 1;
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
        private void OnDamageTakenInvoke(int damage)
        {
            m_OnDamageTaken?.Invoke(damage);
            Debug.Log($"Damage Taken: {damage}");
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
        protected virtual void TakeDamageInternal(int damage)
        {
            if (!IsProtectGoneInternal())
            {
                if (m_Barrier > 0)
                {
                    //m_Barrier--;
                    AddBarrierInternal(-1);
                    //OnBarrierChangedInvoke(m_Barrier);
                    
                }
                if (m_Shield > 0)
                {
                    //m_Shield -= damage;
                    AddShieldInteral(-damage);
                    //OnShieldChangedInvoke(m_Shield);
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
            OnDamageTakenInvoke(damage);
        }
        public void AddHealth(int val)
        {
            AddHealthInternal(val);
        }
        public void AddCurrentHealth(int val)
        {
            AddCurrentHealthInternal(val);
        }
        public void AddShield(int val)
        {
            AddShieldInteral(val);
        }
        public void AddBarrier(int val)
        {
            AddBarrierInternal(val);
        }
        public void SetHealthInternal(int val)
        {
            m_Health = val;
            m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, m_Health);
            OnHealthChangedInvoke(m_CurrentHealth);
            OnHealthRateChangedInvoke(GetHealthRateInternal());
        }
        public void SetHealth(int set)
        {
            SetHealthInternal(set);
        }
        public void SetShield(int val)
        {
            SetShieldInternal(val);
        }
        public void SetBarrier(int val)
        {
            SetBarrierInternal(val);
        }
        protected virtual void AddHealthInternal(int val)
        {
            m_Health += val;
            m_CurrentHealth += val;
            ClampHealth();
        }
        protected virtual void AddCurrentHealthInternal(int val)
        {
            m_CurrentHealth += val;
            ClampHealth();
            OnHealthChangedInvoke(m_CurrentHealth);
        }
        protected virtual void AddShieldInteral(int val)
        {
            m_Shield += val;
            if (m_Shield < 0)
            {
                //m_Shield = 0;
                SetShieldInternal(0);
            }
            OnShieldChangedInvoke(m_Shield);
        }
        protected virtual void AddBarrierInternal(int val)
        {
            m_Barrier += val;
            if (m_Barrier < 0)
            {
                SetBarrierInternal(0);
            }
            OnBarrierChangedInvoke(m_Barrier);
        }
        protected virtual void SetShieldInternal(int val)
        {
            m_Shield = val;
            OnShieldChangedInvoke(m_Shield);
        }
        protected virtual void SetBarrierInternal(int val)
        {
            m_Barrier = val;
            OnBarrierChangedInvoke(m_Barrier);
        }
        protected void HealInternal(int val)
        {
            m_CurrentHealth += val;
            ClampHealth();
            OnHealthChangedInvoke(m_CurrentHealth);
        }

        protected virtual void DeathHandler()
        {
            if (!IsAlive())
            {
                OnDeathInvoke();
            }
        }
        protected virtual void OnDeathInvoke()
        {
            m_OnDeath?.Invoke();
        }
        protected bool IsAlive()
        {
            return m_CurrentHealth > 0;
        }
    }
}
