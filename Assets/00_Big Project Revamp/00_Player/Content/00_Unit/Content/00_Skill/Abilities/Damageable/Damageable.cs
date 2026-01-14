using LegionKnight;
using MoreMountains.Tools;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class Damageable : MonoBehaviour
    {
        [SerializeField, Tooltip("How many times this unit can reborn after death")]
        private int m_RebornCount = 0;

        [SerializeField, MMReadOnly]
        private int m_RemainingReborn;
        
        [SerializeField]
        private int m_MaxHealth = 100;
        [SerializeField]
        private int m_Health = 100;
        [SerializeField]
        private int m_Defense = 0;
        [SerializeField]
        private int m_Shield = 0;
        [SerializeField]
        private int m_Barrier = 0;
        [SerializeField]
        private float m_DamageReductionRate;

        [SerializeField, MMReadOnly]
        private int m_CurrentDamageTaken;
        [SerializeField, MMReadOnly]
        private int m_TotalDamageTaken;
        [SerializeField, MMReadOnly]
        private bool m_IsInvicible = false;
        [SerializeField]
        private UnityEvent m_OnRestart;
        [SerializeField]
        private UnityEvent<HealerContext> m_OnHealed;
        [SerializeField]
        private UnityEvent<BattleContext> m_OnHit;
        [SerializeField]
        private UnityEvent<BattleContext> m_OnDamageTaken;
        [SerializeField]
        private UnityEvent<BattleContext> m_OnDeath;
        [SerializeField]
        private UnityEvent<int> m_OnStartReborn;
        private const int m_MinimumDefendReduction = 0;
        [SerializeField, MMReadOnly]
        private AbilityContext m_AbilityContext;
        public int RemainingReborn => m_RemainingReborn;
        public int MaxHealth => m_MaxHealth;
        public int Health => m_Health;
        public int Defense => m_Defense;
        public int Shield => m_Shield;
        public int Barrier => m_Barrier;
        public float DamageReductionRate => m_DamageReductionRate;
        public int CurrentDamageTaken => m_CurrentDamageTaken;
        public int TotalDamageTaken => m_TotalDamageTaken;
        public UnityEvent<BattleContext> OnHit => m_OnHit;
        public float CurrentHealthRate
        {
            get
            {
                float currentRate = (float)m_Health / m_MaxHealth;
                return currentRate;
            }
        }
        public bool IsInvicible => m_IsInvicible;
        public AbilityContext AbilityContext => m_AbilityContext;

        public void Init(AbilityContext context)
        {
            m_AbilityContext = context;
            m_RebornCount = context.SkillContext.ModuleContext.UnitOwner.Config.RebornCount;
            m_RemainingReborn = m_RebornCount;
            ReborInternal(1f);
        }
        public void Reborn(float healthRate) // change to rebornContext if too many argument in the future
        {
            ReborInternal(healthRate);
        }
        private void ReborInternal(float healthRate)
        {
            if (m_AbilityContext != null)
            {
                Unit ownerObject = m_AbilityContext.SkillContext.ModuleContext.UnitOwner;
                int ownerLevel = ownerObject.Progression.Level;
                float healthFinal = Mathf.Max(0f, ownerObject.Config.MainStats.GetFinalStat(ownerLevel).Health);
                float defenseFinal = Mathf.Max(0f, ownerObject.Config.MainStats.GetFinalStat(ownerLevel).Defense);
                m_MaxHealth = Mathf.RoundToInt(healthFinal);
                m_Health = Mathf.RoundToInt(healthFinal * healthRate);
                m_Defense = Mathf.RoundToInt(defenseFinal);

                m_OnRestart?.Invoke();
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Attacker attacker))
            {
                if (AbilityUltility.IsTargetAllowedByTargetObject(m_AbilityContext.AbilityDeliver, GetComponent<Targetable>()))
                {
                    TakeDamageInternal(attacker);
                }
            }
        }
        public void TakeDamage(Attacker attacker)
        {
            TakeDamageInternal(attacker);
        }
        protected virtual void TakeDamageInternal(Attacker attacker)
        {
            BattleContext context = new BattleContext(attacker, this);
            int effectiveDamage = context.DamageFormulaRPG();
            m_OnHit?.Invoke(context);
            if (m_IsInvicible) return;
            // block damage if barrier exist
            if (m_Barrier > 0)
            {
                AddBarrierInternal(-1);
                return;
            }
            // Apply remaining damage to Shield
            if (effectiveDamage > 0f && m_Shield > 0)
            {
                int previousShield = m_Shield;
                AddShieldInternal(-effectiveDamage);
                if (m_Shield < 0)
                {
                    effectiveDamage = -previousShield;
                    m_Shield = 0;
                }
                else
                {
                    effectiveDamage = 0;
                }
            }
            // Apply remaining damage to Health
            if (effectiveDamage > 0f)
            {
                AddTotalDamageTakeInternal(effectiveDamage);
                AddHealthInternal(-effectiveDamage);
                SetCurrentDamageTakeInternal(effectiveDamage);

                if (m_Health <= 0)
                {
                    OnHealthDepleted(context);
                }
            }
            OnDamageTaken(context);
        }

        public void Heal(Healer healer)
        {
            HealerContext context = new HealerContext(healer, this);
            AddHealthInternal(healer.HealAmount);
            m_OnHealed?.Invoke(context);
        }
        private void OnHealthDepleted(BattleContext context)
        {
            m_OnDeath?.Invoke(context); // Targetable will set IsAlive = false here

            if (m_RemainingReborn > 0)
            {
                AddRemaininRebornCountInternal(-1);
                StartCoroutine(RebornDelayRoutine(1f)); // optional delay
            }
        }
        private IEnumerator RebornDelayRoutine(float delay)
        {
            m_OnStartReborn?.Invoke(m_RemainingReborn);
            yield return new WaitForSeconds(delay);
            ReborInternal(1f);
        }
        private void OnDamageTaken(BattleContext context)
        {
            m_OnDamageTaken?.Invoke(context);
            OnDeathInvoke(context);
        }
        private void OnDeathInvoke(BattleContext context)
        {
            m_OnDeath?.Invoke(context);
        }
        protected virtual void SetCurrentDamageTakeInternal(int damage)
        {
            m_CurrentDamageTaken = damage;
        }
        protected virtual void AddTotalDamageTakeInternal(int damage)
        {
            m_TotalDamageTaken += damage;
            m_TotalDamageTaken = Mathf.Clamp(m_TotalDamageTaken, 0, m_MaxHealth);
        }
        protected virtual void AddHealthInternal(int amount)
        {
            m_Health += amount;
            m_Health = Mathf.Max(0, m_Health);
        }
        protected virtual void AddDefenseInternal(int amount)
        {
            m_Defense += amount;
            m_Defense = Mathf.Max(m_MinimumDefendReduction, m_Defense);
        }
        protected virtual void AddShieldInternal(int amount)
        {
            m_Shield += amount;
            m_Shield = Mathf.Max(0, m_Shield);
        }
        protected virtual void AddBarrierInternal(int amount)
        {
            m_Barrier += amount;
            m_Barrier = Mathf.Max(0, m_Barrier);
        }
        protected virtual void AddDamageReductionRateInternal(float rate)
        {
            m_DamageReductionRate += rate;
        }
        protected virtual void AddRemaininRebornCountInternal(int count)
        {
            m_RemainingReborn += count;
        }
        public void SetInvicible(bool inv)
        {
            m_IsInvicible = inv;
        }
    }
}
