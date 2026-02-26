
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
        private int m_MaxReborn = 0;

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
        private UnityEvent<IAbilityContext> m_OnHit;
        [SerializeField]
        private UnityEvent<IAbilityContext> m_OnDamageTaken;
        [SerializeField]
        private UnityEvent<IAbilityContext> m_OnDeath;
        public UnityEvent<IAbilityContext> OnDeath => m_OnDeath;

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

        public bool IsAlive => m_Health >= 0;
        public Transform TargetTransform => gameObject.transform;

        public void Init(Unit unitOwner)
        {
            m_ModuleContext = new ModuleContext(unitOwner, gameObject);
            m_MaxReborn = unitOwner.Config.RebornCount;
            m_RemainingReborn = m_MaxReborn;
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
                IAbilityDeliver abilityDeliver = attacker.AbilityContext.AbilityDeliver;
                bool isAllowed = AbilityUltility.IsTargetAllowedByTargetObject(abilityDeliver, this);
                if (isAllowed)
                {
                    TakeDamageInternal(attacker.AbilityContext);
                    attacker.OnAttackDelivered.Invoke(this);
                    Debug.Log($"Take Damage from {attacker.AbilityContext.AbilityDeliver.AbilityConfig.BaseInfo.Name}");
                }
                //Debug.Log($"Take Damage from {attacker.AbilityContext.AbilityDeliver.AbilityConfig.BaseInfo.Name}");
            }
            if (collision.TryGetComponent(out IHealer healer))
            {
                IAbilityDeliver abilityDeliver = healer.AbilityContext.AbilityDeliver;
                if (AbilityUltility.IsTargetAllowedByTargetObject(abilityDeliver, this))
                {
                    HealInternal(healer);
                }
            }
            //Debug.Log($"Take Damage from");
        }
        public void TakeDamage(IHasAttacker attacker)
        {
            TakeDamageInternal(attacker.AbilityContext);
        }
        protected virtual void TakeDamageInternal(IAbilityContext attackerContext)
        {
            int effectiveDamage = DamageUtility.DamageFormulaRPG(AbilityUltility.GetAttacker(attackerContext), this);
            OnHitInvoke(attackerContext);
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
                if (m_Shield <= effectiveDamage)
                {
                    effectiveDamage -= m_Shield;
                    SetShieldInternal(0, 0f);
                }
                else
                {
                    effectiveDamage = 0;
                }
                AddShieldInternal(-effectiveDamage);
            }
            // Apply remaining damage to Health
            if (effectiveDamage > 0f)
            {
                AddTotalDamageTakeInternal(effectiveDamage);
                AddHealthInternal(-effectiveDamage);
                SetCurrentDamageTakeInternal(effectiveDamage);
            }
            OnDamageTaken(attackerContext);
        }



        public void Heal(IHealer healer)
        {
            HealInternal(healer);
        }
        private void HealInternal(IHealer healer)
        {
            AddHealthInternal(healer.HealAmount);
            Unit targetHeal = m_ModuleContext.Unit;
            if (targetHeal == null) return;
            AbilityUltility.OnAbilityDeliveredInvoke(healer.AbilityContext, targetHeal);
            GameObject healerModule = healer.AbilityContext.SkillContext.ModuleContext.Module;
            if (healerModule.TryGetComponent(out IHasSkills healerSkill))
            {
                AbilityUltility.OnSkillEventActivates(healerSkill, ForceActiveState.OnHealing);
            }
            if (targetHeal.HasBind(out IHasSkills healedSkills))
            {
                AbilityUltility.OnSkillEventActivates(healedSkills, ForceActiveState.OnHealed);
            }
            m_OnHealed?.Invoke(new HealerContext(healer, this));
        }
        private void OnHealthDepleted(IAbilityContext context)
        {
            // Masih punya reborn? -> reborn saja, JANGAN OnDeath
            if (m_RemainingReborn > 0)
            {
                AddRemaininRebornCountInternal(-1);
                RushGameManager.Instance.StartCoroutine(RebornDelayRoutine(1f));
                return;
            }

            // Reborn habis -> baru benar-benar mati
            OnDeathInvoke(context);
        }
        private IEnumerator RebornDelayRoutine(float delay)
        {
            m_OnRebornStart?.Invoke(m_RemainingReborn);
            yield return new WaitForSeconds(delay);
            ReborInternal(1f);
        }
        private void OnHitInvoke(IAbilityContext context)
        {
            m_OnHit?.Invoke(context);
            Unit unitTaker = m_ModuleContext.Unit;

            AbilityUltility.OnAbilityDeliveredInvoke(context, unitTaker);

            GameObject hitterModule = context.SkillContext.ModuleContext.Module;
            if (hitterModule.TryGetComponent(out IHasSkills hitterSkill))
            {
                AbilityUltility.OnSkillEventActivates(hitterSkill, ForceActiveState.OnHit);
            }

            if (unitTaker.HasBind(out IHasSkills hasSkill))
            {
                AbilityUltility.OnSkillEventActivates(hasSkill, ForceActiveState.OnGetHit);
            }
        }

        private void OnDamageTaken(IAbilityContext context)
        {
            m_OnDamageTaken?.Invoke(context);
            Unit unitTaker = m_ModuleContext.Unit;
            if (unitTaker == null)
            {
                return;
            }
            GameObject damagerModule = context.SkillContext.ModuleContext.Module;
            if (damagerModule.TryGetComponent(out IHasSkills damagerSkill))
            {
                AbilityUltility.OnSkillEventActivates(damagerSkill, ForceActiveState.OnDamageDealed);
            }

            if (unitTaker.HasBind(out IHasSkills hasSkill))
            {
                AbilityUltility.OnSkillEventActivates(hasSkill, ForceActiveState.OnDamageTaken);
            }

            if (m_Health <= 0)
            {
                OnHealthDepleted(context);
            }
            OnDeathInvoke(context);
        }
        private void OnDeathInvoke(IAbilityContext context)
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
        protected virtual void SetHealthInternal(int amount)
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
                SetHealthInternal(amount);
            }
        }
        protected virtual void SetDefenseInternal(int amount)
        {
            m_Defense = amount;
        }
        protected virtual void MultiplyDefenseInternal(float val)
        {
            m_Defense = Mathf.RoundToInt(m_Defense * val);
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
            SetHealthInternal(val);
        }

        public void SetMaxHealth(int val, bool withCurrentHealth)
        {
            SetMaxHealthInternal(val, withCurrentHealth);
        }

        public void AddHealth(int val)
        {
            AddHealthInternal(val);
        }

        public void AddMaxHealth(int val, bool withCurrentHealth)
        {
            AddMaxHealthInternal(val, withCurrentHealth);
        }

        protected virtual void MultiplyHealthInternal(float val)
        {
            m_Health = Mathf.RoundToInt(m_Health * val);
        }
        protected virtual void MultiplyMaxHealthInternal(float val, bool withCurrentHealth)
        {
            m_MaxHealth = Mathf.RoundToInt(m_MaxHealth * val);
            if (withCurrentHealth)
            {
                MultiplyHealthInternal(val);
            }
        }

        public void MultiplyHealth(int val)
        {
            MultiplyHealthInternal(val);
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
            MultiplyDefenseInternal(defense);
        }

        public void Notify(AbilityContext context)
        {
            TakeDamageInternal(context);
        }

        public void SetTargeted(bool targeted)
        {
            m_IsTargeted = targeted;
        }
        
    }
}
