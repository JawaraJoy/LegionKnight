using MoreMountains.Tools;
using NaughtyAttributes;
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
        protected int m_Defend;
        [SerializeField]
        protected int m_Health;
        [SerializeField]
        private int m_Shield;
        [SerializeField]
        private int m_Barrier;
        protected int m_CurrentHealth;

        [SerializeField]
        private float m_MaxHpRateDamage = 0f;

        [SerializeField]
        private bool m_Fatal = false; // If true, the damageable will die on contact with a fatal damage
        [SerializeField]
        private bool m_InstantDeath = false;
        [SerializeField, ReadOnly]
        private bool m_Immortal = false; // If true, the damageable is immortal and cannot
        [SerializeField]
        private UnityEvent m_OnDeath = new();

        [SerializeField]
        private UnityEvent<int> m_OnDamageTaken = new();
        [SerializeField]
        private UnityEvent<int> m_OnDefendChanged = new();
        [SerializeField]
        private UnityEvent<float> m_OnHealthRateChanged = new();
        [SerializeField]
        private UnityEvent<int> m_OnDamageChanged = new();
        [SerializeField]
        private UnityEvent<int> m_OnShieldChanged = new();
        [SerializeField]
        private UnityEvent<int> m_OnBarrierChanged = new();
        [SerializeField]
        private UnityEvent<int> m_OnHealthChanged = new();
        [SerializeField]
        private UnityEvent m_OnProtectGone = new();
        public float MaxHpRateDamage => m_MaxHpRateDamage;
        public bool IsImmortal => m_Immortal;
        public bool IsFatal => m_Fatal;
        protected override void OnContactedBehaviourInvoke(GameObject other)
        {
            base.OnContactedBehaviourInvoke(other);

            if (other.TryGetComponent(out Damageable projectile))
            {
                TakeDamageInternal(projectile);
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
        public void SetFatal(bool fatal)
        {
            m_Fatal = fatal;
            Debug.Log($"Set Fatal: {m_Fatal}");
        }
        public int Damage => m_Damage;
        public int Health => m_Health;
        public int Shield => m_Shield;
        public int CurrentHealth => m_CurrentHealth;
        public int Barrier => m_Barrier;

        public int Defend => m_Defend;

        private float GetHealthRateInternal()
        {
            return (float)m_CurrentHealth / (float)m_Health;
        }
        private void ClampHealth()
        {
            m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, m_Health);
            OnHealthRateChangedInvoke(GetHealthRateInternal());
        }
        private void OnDamageChangedInvoke(int damage)
        {
            m_OnDamageChanged?.Invoke(damage);
            Debug.Log($"Damage Changed: {damage}");
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
        private void OnDefendChangedInvoke(int defend)
        {
            m_OnDefendChanged?.Invoke(defend);
            Debug.Log($"Defend Changed: {defend}");
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
        public void SetImmotral(bool immortal)
        {
            m_Immortal = immortal;
            if (immortal)
            {
                Debug.Log("Damageable is now immortal.");
            }
            else
            {
                Debug.Log("Damageable is no longer immortal.");
            }
        }
        public void TakeDamage(int damage)
        {
            TakeDamageInternal(damage, false);
        }

        private int DamageFormulaRPG(int attacker, int defender)
        {
            int underAmor = Mathf.Clamp(attacker + defender, 5, int.MaxValue);
            int dmg = Mathf.RoundToInt( attacker * attacker / (underAmor));
            if (dmg < 1)
            {
                dmg = Random.Range(1, 5); // Ensure at least 1 damage is dealt
            }
            return dmg;
        }
        private int DamageFormulaMoba(int attacker, int defender)
        {
            int underAmor = Mathf.Clamp(100 + defender, 5, int.MaxValue);
            int dmg = Mathf.RoundToInt(attacker * 100 / (underAmor));
            if (dmg < 1)
            {
                dmg = Random.Range(1, 5); // Ensure at least 1 damage is dealt
            }
            return dmg;
        }
        protected virtual void TakeDamageInternal(Damageable damageable)
        {
            if (damageable == null) return;
            int flatDamage = damageable.Damage;
            float maxHealthRateDamage = damageable.MaxHpRateDamage * m_Health;
            int damage = Mathf.RoundToInt(flatDamage + maxHealthRateDamage);
            if (damageable.m_InstantDeath)
            {
                //m_Barrier = 0;
                //m_Shield = 0;
                OnDamageTakenInvoke(m_CurrentHealth);
                OnDeathInvoke();
                return;
            }
            if (damageable.IsFatal)
            {
                damage = m_CurrentHealth; // Set damage to current health if fatal
            }
            TakeDamageInternal(damage, damageable.IsFatal);
        }
        protected virtual void TakeDamageInternal(int damage, bool fatal)
        {
            //if (!IsAlive()) return;
            if (m_Immortal)
            {
                Debug.Log("Damageable is immortal, no damage taken.");
                return;
            }
            
            int dmg = DamageFormulaMoba(damage, m_Defend);
            if (fatal)
            {
                //m_Barrier = 0;
                dmg = damage; // Set damage to current health if fatal
            }
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
                    AddShieldInteral(-dmg);
                    //OnShieldChangedInvoke(m_Shield);
                }
            }
            else
            {
                m_CurrentHealth -= dmg;
                OnHealthChangedInvoke(m_CurrentHealth);
                ClampHealth();
            }
            if (IsProtectGoneInternal())
            {
                OnProtectGoneInvoke();
            }
            DeathHandler();
            OnDamageTakenInvoke(dmg);
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
            m_CurrentHealth = m_Health; // Reset current health to max health
            ClampHealth();
            OnHealthChangedInvoke(m_CurrentHealth);

            Debug.Log($"Health Set: {m_Health}, Current Health: {m_CurrentHealth}");
        }
        public void SetHealth(int set)
        {
            SetHealthInternal(set);
        }
        public void SetShield(int val)
        {
            SetShieldInternal(val);
        }
        public void SetDefend(int val)
        {
            SetDefendInternal(val);
        }
        public void SetDamage(int val)
        {
            SetDamageInternal(val);
        }
        private void SetDamageInternal(int val)
        {
            m_Damage = val;
            OnDamageChangedInvoke(m_Damage);
        }
        private void SetDefendInternal(int val)
        {
            m_Defend = val;
            OnDefendChangedInvoke(m_Defend);
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

        public void RestartHealth()
        {
            m_CurrentHealth = m_Health;
            OnHealthChangedInvoke(m_CurrentHealth);
            OnHealthRateChangedInvoke(GetHealthRateInternal());
            OnShieldChangedInvoke(m_Shield);
            OnBarrierChangedInvoke(m_Barrier);
        }
    }
}
