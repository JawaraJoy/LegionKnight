
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
        [SerializeField]
        private bool m_IsImmortal = false;
        [SerializeField]
        private bool m_IsInvisible = false;

        
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
        private UnityEvent<int> m_OnSimpleDamageTaken;
        [SerializeField]
        private UnityEvent<IAbilityContext> m_OnDeath;
        [SerializeField]
        private UnityEvent<Transform> m_OnDeathDirection;
        public UnityEvent<IAbilityContext> OnDeath => m_OnDeath;

        [SerializeField]
        private UnityEvent<int, int> m_OnHealthChanged;
        [SerializeField]
        private UnityEvent<int, int> m_OnShieldChanged;
        [SerializeField]
        private UnityEvent<bool> m_OnShieldExisted;
        [SerializeField]
        private UnityEvent<int> m_OnBarrierChanged;
        [SerializeField]
        private UnityEvent<int> m_OnBarrierAdded;
        [SerializeField]
        private UnityEvent<int> m_OnBarrierRemoved;
        [SerializeField]
        private UnityEvent<int> m_OnRebornChanged;
        [SerializeField]
        private UnityEvent<int> m_OnRebornAdded;
        [SerializeField]
        private UnityEvent<int> m_OnRebornRemoved;
        [SerializeField]
        private UnityEvent<bool> m_OnImmortalChanged;
        [SerializeField]
        private UnityEvent<float> m_OnImmortalDurationSet;
        [SerializeField]
        private UnityEvent<bool> m_OnInvisibleChanged;
        [SerializeField]
        private UnityEvent<float> m_OnInvisibleDurationSet;

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
        public bool IsInvisible => m_IsInvisible;

        private ModuleContext m_ModuleContext;
        public IModuleContext ModuleContext => m_ModuleContext;

        public bool IsTargeted => m_IsTargeted;

        public bool IsAlive => m_Health > 0;
        public Transform TargetTransform => gameObject.transform;

        public void Init(Unit unitOwner)
        {
            m_ModuleContext = new ModuleContext(unitOwner, gameObject);
            SetRemainingRebornCountInternal(unitOwner.Config.RebornCount);
            ReborInternal(1f); // always reborn in 100% health
        }
        public void RefreshDamageableStat(float healRate, bool currentHealthRefresh)
        {
            RefreshDamageableStatInternal(healRate, currentHealthRefresh);
        }
        private void RefreshDamageableStatInternal(float healRate, bool currentHealthRefresh)
        {
            Unit unit = m_ModuleContext.Unit;
            int ownerLevel = unit.Progression.Level;
            StatField ownerStat = unit.Config.MainStats.GetFinalStat(ownerLevel);

            if (unit.HasBind(out StatController controller))
            {
                ownerStat = controller.GetFinalStat(ownerStat);
            }
            float healthFinal = Mathf.Max(0f, ownerStat.Health);
            float defenseFinal = Mathf.Max(0f, ownerStat.Defense);

            SetMaxHealthInternal(Mathf.RoundToInt(healthFinal));
            if (currentHealthRefresh) 
            {
                SetHealthInternal(Mathf.RoundToInt(healthFinal * healRate));
            }
            SetDefenseInternal(Mathf.RoundToInt(defenseFinal));
        }
        public void Reborn(float healthRate, int fixedShield = 0, int barrier = 0, float InvisibilityDuration = 2f) // change to rebornContext if too many argument in the future
        {
            ReborInternal(healthRate, fixedShield, barrier, InvisibilityDuration);
        }
        private void ReborInternal(float healthRate, int fixedShield = 0, int barrier = 0, float InvisibilityDuration = 2f)
        {
            InvisibleForWhileInternal(InvisibilityDuration);

            RefreshDamageableStatInternal(healthRate, true);

            SetShieldInternal(fixedShield);
            SetBarrierInternal(barrier);
            m_OnRebornDone?.Invoke();
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
        protected virtual void TakeDamageInternal(IAbilityContext context)
        {
            if (m_IsInvisible)
                return;

            AttackerField attacker = AbilityUltility.GetAttacker(context);

            int rawDamage = DamageUtility.CalculateRawDamage(attacker, this);
            int reducedDamage = DamageUtility.ApplyDamageReduction(rawDamage, m_DamageReductionRate);

            OnHitInvoke(context);

            int finalDamage = ApplyProtectionLayers(reducedDamage);

            if (finalDamage > 0)
            {
                ApplyDamage(finalDamage);
            }

            OnDamageTaken(context, finalDamage);
        }
        private int ApplyProtectionLayers(int damage)
        {
            if (damage <= 0)
                return 0;

            // Barrier block 1 hit
            if (m_Barrier > 0)
            {
                AddBarrierInternal(-1, true);
                return 0;
            }

            // Shield absorb value
            if (m_Shield > 0)
            {
                int absorbed = Mathf.Min(m_Shield, damage);
                AddShieldInternal(-absorbed, true);
                damage -= absorbed;
            }

            return damage;
        }
        protected virtual void ApplyDamage(int damage)
        {
            if (damage <= 0)
                return;

            SetCurrentDamageTakeInternal(damage);
            AddTotalDamageTakeInternal(damage);

            m_Health -= damage;

            // Immortal tidak boleh mati
            if (m_IsImmortal && m_Health <= 0)
            {
                m_Health = 1;
            }

            m_Health = Mathf.Clamp(m_Health, 0, m_MaxHealth);

            m_OnHealthChanged?.Invoke(m_Health, m_MaxHealth);
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
                AddRemaininRebornCountInternal(-1, true);
                
                //StartCoroutine(RebornDelayRoutine(1f));
                ReborInternal(1f);
                Debug.Log("Reborned");
                return;
            }

            // Reborn habis -> baru benar-benar mati
            OnDeathInvoke(context);
        }
        private IEnumerator RebornDelayRoutine(float delay)
        {
            SetInvisibleInternal(true);
            m_OnRebornStart?.Invoke(m_RemainingReborn);
            yield return new WaitForSeconds(delay);
            ReborInternal(1f);
            Debug.Log("Reborned");
            SetInvisibleInternal(false);
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

        private void OnSimpleDamageTakenInvoke(int damage)
        {
            m_OnSimpleDamageTaken?.Invoke(damage);
        }
        private void OnDamageTaken(IAbilityContext context, int finalAmount)
        {
            m_OnDamageTaken?.Invoke(context);
            OnSimpleDamageTakenInvoke(finalAmount);
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
            //OnDeathInvoke(context);
        }
        private void OnDeathInvoke(IAbilityContext context)
        {
            m_OnDeath?.Invoke(context);
            m_OnDeathDirection?.Invoke(context.AbilityDeliver.DeliverTransform);
        }
        protected virtual void SetCurrentDamageTakeInternal(int damage)
        {
            m_CurrentDamageTaken = damage;
        }
        protected virtual void AddTotalDamageTakeInternal(int damage)
        {
            m_TotalDamageTaken += damage;
            m_TotalDamageTaken = Mathf.Max(0, m_TotalDamageTaken);
            Debug.Log($"Total Damage Taken: {m_TotalDamageTaken}, curren take {damage}");
        }
        protected virtual void AddHealthInternal(int amount)
        {
            m_Health += amount;
            m_Health = Mathf.Clamp(m_Health, 0, m_MaxHealth);
            m_OnHealthChanged?.Invoke(m_Health, m_MaxHealth);
        }
        protected virtual void AddDefenseInternal(int amount)
        {
            m_Defense += amount;
            m_Defense = Mathf.Max(m_MinimumDefendReduction, m_Defense);
        }
        public void AddShield(int amount, bool callAddRemoveEvent)
        {
            AddShieldInternal(amount, callAddRemoveEvent);
        }
        protected virtual void AddShieldInternal(int amount, bool callAddRemoveEvent)
        {
            m_Shield += amount;
            m_Shield = Mathf.Clamp(m_Shield, 0, m_MaxHealth);
            
            m_OnShieldChanged?.Invoke(m_Shield, m_MaxHealth);
            m_OnShieldExisted?.Invoke(m_Shield > 0);
            if (!callAddRemoveEvent) return;
            // call add remove event
        }

        public void AddBarrier(int amount, bool callAddRemoveEvent)
        {
            AddBarrierInternal(amount, callAddRemoveEvent);
        }
        protected virtual void AddBarrierInternal(int amount, bool callAddRemoveEvent)
        {
            m_Barrier += amount;
            m_Barrier = Mathf.Max(0, m_Barrier);
            
            m_OnBarrierChanged?.Invoke(m_Barrier);
            if (!callAddRemoveEvent) return;
            if (amount > 0)
            {
                m_OnBarrierAdded?.Invoke(amount);
            }
            else if (amount < 0)
            {
                m_OnBarrierRemoved?.Invoke(amount);
            }
        }
        public void AddDamageReductionRate(float rate)
        {
            AddDamageReductionRateInternal(rate);
        }
        protected virtual void SetBarrierInternal(int amount)
        {
            m_Barrier = amount;
            m_Barrier = Mathf.Max(0, m_Barrier);
            m_OnBarrierChanged?.Invoke(m_Barrier);
        }
        
        public void SetRemainingRebornCount(int count)
        {
            SetRemainingRebornCountInternal(count);
        }
        public void AddRemainingRebornCount(int count, bool callAddRemoveEvent)
        {
            AddRemaininRebornCountInternal(count, callAddRemoveEvent);
        }
        private void SetRemainingRebornCountInternal(int count)
        {
            m_RemainingReborn = count;
            m_OnRebornChanged?.Invoke(m_RemainingReborn);
        }
        protected virtual void AddDamageReductionRateInternal(float rate)
        {
            m_DamageReductionRate += rate;
            m_DamageReductionRate = Mathf.Clamp01(m_DamageReductionRate);
        }
        protected virtual void AddRemaininRebornCountInternal(int count, bool callAddRemoveEvent)
        {
            m_RemainingReborn += count;
            m_RemainingReborn = Mathf.Clamp(m_RemainingReborn, 0, int.MaxValue);
            
            m_OnRebornChanged?.Invoke(m_RemainingReborn);
            if (!callAddRemoveEvent) return;
            if (count > 0)
            {
                m_OnRebornAdded?.Invoke(count);
            }
            else if (count < 0)
            {
                m_OnRebornRemoved?.Invoke(count);
            }
        }
        protected virtual void AddMaxHealthInternal(int amount, bool restoreCurrent)
        {
            m_MaxHealth += amount;
            if (restoreCurrent)
            {
                AddHealthInternal(amount);
            }
            m_OnHealthChanged?.Invoke(m_Health, m_MaxHealth);
        }
        protected virtual void SetHealthInternal(int amount)
        {
            m_Health = amount;
            if (m_Health > m_MaxHealth)
            {
                m_Health = m_MaxHealth;
            }
            m_OnHealthChanged?.Invoke(m_Health, m_MaxHealth);
        }
        protected virtual void SetMaxHealthInternal(int amount)
        {
            m_MaxHealth = amount;
            if (m_Health > m_MaxHealth)
            {
                m_Health = m_MaxHealth;
            }
            m_OnHealthChanged?.Invoke(m_Health, m_MaxHealth);
        }
        protected virtual void SetDefenseInternal(int amount)
        {
            m_Defense = amount;
        }
        protected virtual void MultiplyDefenseInternal(float val)
        {
            m_Defense = Mathf.RoundToInt(m_Defense * val);
        }

        protected virtual void SetShieldInternal(int amount)
        {
            m_Shield = amount;
            m_Shield = Mathf.Clamp(amount, 0, m_MaxHealth);
            m_OnShieldChanged?.Invoke(m_Shield, m_MaxHealth);
            m_OnShieldExisted?.Invoke(m_Shield > 0);
        }
        
        public void SetImmortal(bool imo)
        {
            SetImmortalInternal(imo);
        }
        protected virtual void SetImmortalInternal(bool inv)
        {
            m_IsImmortal = inv;
            m_OnImmortalChanged?.Invoke(m_IsImmortal);
        }

        private void ImmortalForWhileInternal(float duration)
        {
            StartCoroutine(ImmortalingForWhile(duration));
            m_OnImmortalDurationSet?.Invoke(duration);
        }
        private IEnumerator ImmortalingForWhile(float duration)
        {
            SetImmortalInternal(true);
            yield return new WaitForSeconds(duration);
            SetImmortalInternal(false);
        }

        private void InvisibleForWhileInternal(float duration)
        {
            StartCoroutine(InvisiblingForWhile(duration));
            m_OnInvisibleDurationSet?.Invoke(duration);
        }
        private IEnumerator InvisiblingForWhile(float duration)
        {
            SetInvisibleInternal(true);
            yield return new WaitForSeconds(duration);
            SetInvisibleInternal(false);
        }
        public void SetInvisible(bool inv)
        {
            SetInvisibleInternal(inv);
        }
        protected virtual void SetInvisibleInternal(bool inv)
        {
            m_IsInvisible = inv;
            m_OnInvisibleChanged?.Invoke(m_IsInvisible);
        }

        public void SetHealth(int val)
        {
            SetHealthInternal(val);
        }

        public void SetMaxHealth(int val)
        {
            SetMaxHealthInternal(val);
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
