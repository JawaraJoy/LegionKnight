
using MoreMountains.Tools;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class Damageable : MonoBehaviour, IUnitExtension, IDamageable, ITargetable
    {
        [SerializeField]
        private bool m_IsTargeted;
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
        private UnityEvent<CombatContext> m_OnHit;
        [SerializeField]
        private UnityEvent<CombatContext> m_OnDamageTaken;
        [SerializeField]
        private UnityEvent<CombatContext> m_OnDeath;

        private const int m_MinimumDefendReduction = 0;
        public int RemainingReborn => m_RemainingReborn;
        public int MaxHealth => m_MaxHealth;
        public int Health => m_Health;
        public int Defense => m_Defense;
        public int Shield => m_Shield;
        public int Barrier => m_Barrier;
        public float DamageReductionRate => m_DamageReductionRate;
        public int CurrentDamageTaken => m_CurrentDamageTaken;
        public int TotalDamageTaken => m_TotalDamageTaken;
        public UnityEvent<CombatContext> OnHit => m_OnHit;
        public float HealthRate
        {
            get
            {
                float currentRate = (float)m_Health / m_MaxHealth;
                return currentRate;
            }
        }
        public bool IsImmortal => m_IsImmortal;

        private ModuleContext m_ModuleContext;
        public IModuleContext ModuleContext => m_ModuleContext;

        public bool IsTargeted => m_IsTargeted;

        public bool IsAlive => m_Health <= 0;
        public Transform TargetTransform => gameObject.transform;

        public void Init(Unit unitOwner)
        {
            m_ModuleContext = new ModuleContext(unitOwner, gameObject);
            m_RebornCount = 0;
            m_RemainingReborn = 0;
            ReborInternal(1f); // always reborn in 100% health
        }
        public void Reborn(float healthRate, int fixedShield = 0, int barrier = 0, float immortalDuration = 0f) // change to rebornContext if too many argument in the future
        {
            ReborInternal(healthRate, fixedShield, barrier, immortalDuration);
        }
        private void ReborInternal(float healthRate, int fixedShield = 0, int barrier = 0, float immortalDuration = 0f)
        {
            Unit unit = m_ModuleContext.Unit;
            int ownerLevel = unit.Progression.Level;
            float healthFinal = Mathf.Max(0f, unit.Config.MainStats.GetFinalStat(ownerLevel).Health * healthRate);
            float defenseFinal = Mathf.Max(0f, unit.Config.MainStats.GetFinalStat(ownerLevel).Defense);


            SetMaxHealthInternal(Mathf.RoundToInt(healthFinal), true);
            SetDefenseInternal(Mathf.RoundToInt(defenseFinal));
            m_Shield = fixedShield;
            m_Barrier = barrier;
            m_OnRebornDone?.Invoke();

            ImmortalForWhileInternal(immortalDuration);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IHasAttacker attacker))
            {
                if (attacker is IHasAbilityContext attackerContext)
                {
                    IAbilityDeliver abilityDeliver = attackerContext.AbilityContext.AbilityDeliver;
                    if (m_ModuleContext.Unit.HasBind(out ITargetable targetable))
                    {
                        if (AbilityUltility.IsTargetAllowedByTargetObject(abilityDeliver, targetable))
                        {
                            TakeDamageInternal(attacker);
                        }
                    }
                }
                else
                {
                    TakeDamageInternal(attacker);
                }
            }
        }
        public void TakeDamage(IHasAttacker attacker)
        {
            TakeDamageInternal(attacker);
        }
        protected virtual void TakeDamageInternal(IHasAttacker attacker)
        {
            CombatContext combatContext = new CombatContext(attacker, this);
            int effectiveDamage = DamageUtility.DamageFormulaRPG(attacker, this);
            OnHitInvoke(combatContext);
            if (m_IsImmortal) return;
            // block damage if barrier exist
            if (m_Barrier > 0)
            {
                AddBarrierInternal(-1, 0f);
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
                    SetShieldInternal(0, 0f);
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
            OnDamageTaken(combatContext);
        }


        public void Heal(IHealer healer)
        {
            AddHealthInternal(healer.HealAmount);
            if (healer is IHasAbilityContext context)
            {
                Unit targetHeal = m_ModuleContext.Unit;
                if (targetHeal == null) return;
                AbilityUltility.OnAbilityDeliveredInvoke(context.AbilityContext, targetHeal);
                GameObject healerModule = context.AbilityContext.SkillContext.ModuleContext.Module;
                if (healerModule.TryGetComponent(out IHasSkills healerSkill))
                {
                    AbilityUltility.OnSkillEventActivates(healerSkill, SkillTriggerState.OnHealing);
                }
                if (targetHeal.HasBind(out IHasSkills healedSkills))
                {
                    AbilityUltility.OnSkillEventActivates(healedSkills, SkillTriggerState.OnHealed);
                }
            }
            m_OnHealed?.Invoke(new HealerContext(healer, this));
            
            
        }
        private void OnHealthDepleted()
        {
            if (m_RemainingReborn > 0)
            {
                AddRemaininRebornCountInternal(-1);
                RushGameManager.Instance.StartCoroutine(RebornDelayRoutine(1f)); // optional delay
            }
        }
        private IEnumerator RebornDelayRoutine(float delay)
        {
            m_OnRebornStart?.Invoke(m_RemainingReborn);
            yield return new WaitForSeconds(delay);
            ReborInternal(1f);
        }
        private void OnHitInvoke(CombatContext combat)
        {
            m_OnHit?.Invoke(combat);
            // attacker ability delivered here
            if (combat.Attacker is IHasAbilityContext hitter)
            {
                Unit unitTaker = m_ModuleContext.Unit;
                
                AbilityUltility.OnAbilityDeliveredInvoke(hitter.AbilityContext, unitTaker);

                GameObject hitterModule = hitter.AbilityContext.SkillContext.ModuleContext.Module;
                if (hitterModule.TryGetComponent(out IHasSkills hitterSkill))
                {
                    AbilityUltility.OnSkillEventActivates(hitterSkill, SkillTriggerState.OnHit);
                }

                if (unitTaker.HasBind(out IHasSkills hasSkill))
                {
                    AbilityUltility.OnSkillEventActivates(hasSkill, SkillTriggerState.OnGetHit);
                }
            }
        }

        private void OnDamageTaken(CombatContext combat)
        {
            m_OnDamageTaken?.Invoke(combat);
            if (combat.Attacker is IHasAbilityContext hitter)
            {
                Unit unitTaker = m_ModuleContext.Unit;
                if (unitTaker == null)
                {
                    return;
                }
                GameObject damagerModule = hitter.AbilityContext.SkillContext.ModuleContext.Module;
                if (damagerModule.TryGetComponent(out IHasSkills damagerSkill))
                {
                    AbilityUltility.OnSkillEventActivates(damagerSkill, SkillTriggerState.OnDamageDealed);
                }

                if (unitTaker.HasBind(out IHasSkills hasSkill))
                {
                    AbilityUltility.OnSkillEventActivates(hasSkill, SkillTriggerState.OnDamageTaken);
                }
            }
            
            if (m_Health <= 0)
            {
                OnHealthDepleted();
            }
            OnDeathInvoke(combat);
        }
        private void OnDeathInvoke(CombatContext context)
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
        protected virtual void AddBarrierInternal(int amount, float duration)
        {
            RushGameManager.Instance.StartCoroutine(AddingBarrierInternal(amount, duration));
        }
        protected virtual IEnumerator AddingBarrierInternal(int amount, float duration)
        {
            m_Barrier += amount;
            if (duration >= 0)
            {
                yield return new WaitForSeconds(duration);
                if (amount > 0)
                {
                    if (m_Barrier >= amount)
                    {
                        m_Barrier -= amount;
                    }
                }
            }
            m_Barrier = Mathf.Max(0, m_Barrier);
        }
        protected virtual void SetBarrierInternal(int amount, float duration)
        {
            RushGameManager.Instance.StartCoroutine(SettingBarrierInternal(amount, duration));
        }
        protected virtual IEnumerator SettingBarrierInternal(int amount, float duration)
        {
            m_Barrier = amount;
            if (duration >= 0)
            {
                yield return new WaitForSeconds(duration);
                if (amount > 0)
                {
                    if (m_Barrier >= amount)
                    {
                        m_Barrier -= amount;
                    }
                }
            }
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
        protected virtual void SetShieldInternal(int amount, float duration)
        {
            RushGameManager.Instance.StartCoroutine(SettingShieldInternal(amount, duration));
        }
        protected virtual IEnumerator SettingShieldInternal(int amount, float duration)
        {
            m_Shield = amount;
            if (duration >= 0)
            {
                yield return new WaitForSeconds(duration);
                if (amount > 0)
                {
                    if (m_Shield >= amount)
                    {
                        m_Shield -= amount;
                    }
                }
            }
            m_Shield = Mathf.Max(0, m_Shield);
        }
        public void SetImmortal(bool imo)
        {
            SetImmortalInternal(imo);
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

        public void SetHealth(int val)
        {
            throw new System.NotImplementedException();
        }

        public void SetMaxHealth(int val, bool withCurrentHealth)
        {
            throw new System.NotImplementedException();
        }

        public void AddHealth(int val)
        {
            AddHealthInternal(val);
        }

        public void AddMaxHealth(int val, bool withCurrentHealth)
        {
            throw new System.NotImplementedException();
        }

        public void MultiplyHealth(int val)
        {
            throw new System.NotImplementedException();
        }

        public void MultiplyMaxHealth(int val, bool withCurrentHealth)
        {
            
        }

        public void SetDefense(int defense)
        {
            SetDefenseInternal(defense);
        }

        public void AddDefense(int defense)
        {
            AddDefenseInternal(defense);
        }

        public void MultiplyDefense(int defense)
        {
            throw new System.NotImplementedException();
        }

        public void Notify(AbilityContext context)
        {
            throw new System.NotImplementedException();
        }

        public void SetTargeted(bool targeted)
        {
            m_IsTargeted = targeted;
        }
        
    }
}
