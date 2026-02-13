
using LegionKnight;
using MoreMountains.Tools;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public interface IDamageable
    {
        DamageableField DamageableField { get; }
    }

    public class DamageableField
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
        private bool m_IsImmortal = false;
        [SerializeField]
        private UnityEvent<int> m_OnRebornStart;
        [SerializeField]
        private UnityEvent m_OnRebornDone;
        [SerializeField]
        private UnityEvent<HealerContext> m_OnHealed;
        [SerializeField]
        private UnityEvent<BattleContext> m_OnHit;
        [SerializeField]
        private UnityEvent<BattleContext> m_OnDamageTaken;
        [SerializeField]
        private UnityEvent<BattleContext> m_OnDeath;
        
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
        public bool IsInvicible => m_IsImmortal;
        public AbilityContext AbilityContext => m_AbilityContext;

        public void Init(AbilityContext context)
        {
            m_AbilityContext = context;
            UnitConfig unitConfig = context.SkillContext.ModuleContext.UnitOwner.Config;
            if (unitConfig == null) return;
            
            m_Shield = 0;
            m_Barrier = 0;
            m_RebornCount = unitConfig.RebornCount;
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
                float healthFinal = Mathf.Max(0f, ownerObject.Config.MainStats.GetFinalStat(ownerLevel).Health * healthRate);
                float defenseFinal = Mathf.Max(0f, ownerObject.Config.MainStats.GetFinalStat(ownerLevel).Defense);


                SetMaxHealthInternal(Mathf.RoundToInt(healthFinal), true);
                SetDefenseInternal(Mathf.RoundToInt(defenseFinal));

                m_OnRebornDone?.Invoke();

                ImmortalForWhileInternal(2f);
            }
        }
        public void OnTriggerEnter2D(Collider2D collision, Targetable targetable)
        {
            if (collision.TryGetComponent(out Attacker attacker))
            {
                if (AbilityUltility.IsTargetAllowedByTargetObject(m_AbilityContext.AbilityDeliver, targetable))
                {
                    if (targetable.TryGetComponent(out IDamageable damageable))
                    {
                        TakeDamageInternal(attacker, damageable);
                    }
                    
                }
            }
        }
        public void TakeDamage(IAttacker attacker, IDamageable damageable)
        {
            TakeDamageInternal(attacker, damageable);
        }
        protected virtual void TakeDamageInternal(IAttacker attacker, IDamageable damageable)
        {
            BattleContext context = new BattleContext(attacker, damageable);
            int effectiveDamage = DamageUtility.DamageFormulaRPG(attacker, damageable);
            if (m_IsImmortal) return;
            OnHitInvoke(context);
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


            }
            OnDamageTaken(context);
        }


        public void Heal(Healer healer, IDamageable damageable)
        {
            HealerContext context = new(healer, damageable);
            AddHealthInternal(healer.HealAmount);
            m_OnHealed?.Invoke(context);
            if (context.Damageable is Bindable bindable)
            {
                AbilityUltility.OnSkillDeliveredInvoke(m_AbilityContext, bindable, SkillTriggerState.OnHealed);
            }
        }
        private void OnHealthDepleted()
        {
            if (m_RemainingReborn > 0)
            {
                AddRemaininRebornCountInternal(-1)
                RushGameManager.Instance.StartCoroutine(RebornDelayRoutine(1f)); // optional delay
            }
        }
        private IEnumerator RebornDelayRoutine(float delay)
        {
            m_OnRebornStart?.Invoke(m_RemainingReborn);
            yield return new WaitForSeconds(delay);
            ReborInternal(1f);
        }
        private void OnHitInvoke(BattleContext context)
        {
            m_OnHit?.Invoke(context);
            // attacker ability delivered here
            if (context.Damageable is Bindable damageable)
            {
                AbilityUltility.OnSkillDeliveredInvoke(m_AbilityContext, damageable, SkillTriggerState.OnHit);
            }
            
        }
        
        private void OnDamageTaken(BattleContext context)
        {
            m_OnDamageTaken?.Invoke(context);

            if (context.Attacker is Bindable attacker)
            {
                AbilityUltility.OnSkillDeliveredInvoke(m_AbilityContext, attacker, SkillTriggerState.OnDamageDealed);
            }
            if (context.Damageable is Bindable damageable)
            {
                AbilityUltility.OnSkillDeliveredInvoke(m_AbilityContext, damageable, SkillTriggerState.OnDamageTaken);
            }
            if (m_Health <= 0)
            {
                OnHealthDepleted();
            }
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
        protected virtual void AddMaxHealthInternal(int amount, bool restoreCurrent)
        {
            m_MaxHealth += amount;
            if (restoreCurrent)
            {
                AddHealthInternal(amount);
            }
        }
        protected virtual void SetCurrentHealthInternal(int amount)
        {
            m_Health = amount;
            if (m_Health > m_MaxHealth)
            {
                m_Health = m_MaxHealth;
            }
        }
        protected virtual void SetMaxHealthInternal(int amount, bool restoreCurrent)
        {
            m_MaxHealth = amount;
            if (restoreCurrent)
            {
                SetCurrentHealthInternal(amount);
            }
        }
        protected virtual void SetDefenseInternal(int amount)
        {
            m_Defense = amount;
        }
        public void SetImmortal(bool inv)
        {
            SetImmortalInternal(inv);
        }
        protected virtual void SetImmortalInternal(bool inv)
        {
            m_IsImmortal = inv;
        }
        private void ImmortalForWhileInternal(float duration)
        {
            RushGameManager.Instance.StartCoroutine(ImmortalingForWhile(duration));
        }
        private IEnumerator ImmortalingForWhile(float duration)
        {
            SetImmortalInternal(true);
            yield return new WaitForSeconds(duration);
            SetImmortalInternal(false);

        }
    }
}
