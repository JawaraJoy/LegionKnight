using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class SkillActivator : Bindable, IUpdater
    {
        public enum SkillActivationState
        {
            Idle,
            Casting,
            Cooldown,
            Silenced
        }

        [Header("Config")]
        [SerializeField]
        private SkillActivatorConfig m_SkillConfig;
        [SerializeField]
        private ProgressField m_Progression;
        [SerializeField]
        private Transform m_DeliverSpawnPost;

        [Header("Runtime")]
        [SerializeField, MMReadOnly]
        private SkillContext m_Context;
        [SerializeField, MMReadOnly]
        private List<AbilityDeliver> m_Delivers = new();

        [Header("Events")]
        [SerializeField]
        private UnityEvent<SkillContext> m_OnInit;

        [SerializeField]
        private UnityEvent m_OnCastingStart;
        [SerializeField]
        private UnityEvent<float> m_OnCastingDurationUpdate;
        [SerializeField]
        private UnityEvent<int, int> m_OnCastingInterruptUpdate;
        [SerializeField]
        private UnityEvent m_OnCastingSuccess;
        [SerializeField]
        private UnityEvent m_OnCastingFail;

        [SerializeField]
        private UnityEvent<AbilityContext> m_OnActivateIndividu;
        [SerializeField]
        private UnityEvent<SkillContext> m_OnActivates;
        public ProgressField Progression => m_Progression;
        public SkillActivatorConfig SkillConfig => m_SkillConfig;
        public SkillContext Context => m_Context;
        public IReadOnlyList<AbilityDeliver> Delivers => m_Delivers;

        public bool IsActive => gameObject.activeInHierarchy;

        [Header("State")]
        [SerializeField, MMReadOnly]
        private SkillActivationState m_State = SkillActivationState.Idle;

        [Header("Charge")]
        [SerializeField, MMReadOnly]
        private float m_RemainingCharge;

        [Header("Cooldown")]
        [SerializeField, MMReadOnly]
        private float m_RemainingCooldown;

        [Header("Casting")]
        [SerializeField, MMReadOnly]
        private float m_RemainingCastTime;
        [SerializeField, MMReadOnly]
        private int m_CurrentInterruptCount;
        [SerializeField, MMReadOnly]
        private int m_MaxInterruptCount;
        private AbilityDeliver GetAbilityDeliverInternal(string id)
        {
            return m_Delivers.Find(x => x.Config.BaseInfo.Id == id);
        }

        private bool HasAbilityInternal(string id, out AbilityDeliver abilityDeliver)
        {
            abilityDeliver = GetAbilityDeliverInternal(id);
            return abilityDeliver != null;
        }


        #region Unity Lifecycle

        private void Start()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }

        private void OnDestroy()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        #endregion

        #region Init

        public virtual void Init(SkillActivatorConfig skillConfig, ModuleContext ownerContext)
        {
            m_SkillConfig = skillConfig;
            m_Context = new SkillContext(this, ownerContext);

            m_OnInit?.Invoke(m_Context);

            if (m_SkillConfig.Trigger.TriggerState == SkillTriggerState.OnHit)
            {
                if (ownerContext.UnitOwner.HasBind(out Damageable damageable))
                {
                    damageable.OnHit.RemoveAllListeners();
                    damageable.OnHit.AddListener(_ => TryActivate());
                }
            }

            SpawnDelivers();
        }

        #endregion

        #region Update

        public void Tick()
        {
            if (!IsActive)
                return;

            switch (m_State)
            {
                case SkillActivationState.Casting:
                    TickCasting();
                    break;

                case SkillActivationState.Cooldown:
                    TickCooldown();
                    break;
            }
        }

        #endregion

        #region State Helpers

        private void ChangeState(SkillActivationState newState)
        {
            if (m_State == newState)
                return;

            m_State = newState;
        }

        private bool CanActivate()
        {
            return m_State == SkillActivationState.Idle;
        }

        public void SetSilenced(bool silenced)
        {
            ChangeState(silenced
                ? SkillActivationState.Silenced
                : SkillActivationState.Idle);
        }

        #endregion

        #region Charge

        public void AddCharge(float amount)
        {
            if (m_State == SkillActivationState.Silenced)
                return;

            m_RemainingCharge += amount;

            if (m_RemainingCharge >= m_SkillConfig.Trigger.Charge)
            {
                float overflow = m_RemainingCharge - m_SkillConfig.Trigger.Charge;

                if (m_SkillConfig.Trigger.TriggerState == SkillTriggerState.OnChargeFull &&
                    m_SkillConfig.Trigger.AutoActiveOnReady)
                {
                    TryActivateInternal();
                }

                m_RemainingCharge = Mathf.Max(0f, overflow);
            }
        }

        #endregion

        #region Activation Entry

        public void TryActivate()
        {
            TryActivateInternal();
        }

        private void TryActivateInternal()
        {
            if (!CanActivate())
                return;

            if (m_SkillConfig.Casting.CastDuration > 0f)
            {
                StartCasting();
            }
            else
            {
                ExecuteSkill();
            }
        }

        #endregion

        #region Casting

        private void StartCasting()
        {
            ChangeState(SkillActivationState.Casting);

            m_RemainingCastTime = m_SkillConfig.Casting.CastDuration;
            m_CurrentInterruptCount = 0;
            m_MaxInterruptCount = m_SkillConfig.Casting.MaxInterruptCount;

            m_OnCastingStart?.Invoke();
        }

        private void TickCasting()
        {
            m_RemainingCastTime -= Time.deltaTime;
            m_OnCastingDurationUpdate?.Invoke(m_RemainingCastTime / m_SkillConfig.Casting.CastDuration);
            if (m_RemainingCastTime <= 0f)
            {
                CompleteCasting();
            }
        }

        public void InterruptCasting(int amount)
        {
            if (m_State != SkillActivationState.Casting)
                return;

            m_CurrentInterruptCount += amount;

            m_OnCastingInterruptUpdate?.Invoke(
                m_CurrentInterruptCount,
                m_MaxInterruptCount);

            if (m_CurrentInterruptCount >= m_MaxInterruptCount)
            {
                FailCasting();
            }
        }

        private void CompleteCasting()
        {
            m_OnCastingSuccess?.Invoke();
            ExecuteSkill();
        }

        private void FailCasting()
        {
            m_OnCastingFail?.Invoke();

            ResetCastingData();

            if (m_SkillConfig.Casting.CooldownOnCastFail)
            {
                EnterCooldown();
            }
            else
            {
                ChangeState(SkillActivationState.Idle);
            }
        }

        private void ResetCastingData()
        {
            m_RemainingCastTime = 0f;
            m_CurrentInterruptCount = 0;
            m_MaxInterruptCount = 0;
        }

        #endregion

        #region Cooldown

        private void EnterCooldown()
        {
            m_RemainingCooldown = m_SkillConfig.Trigger.Cooldown;
            ChangeState(SkillActivationState.Cooldown);
        }

        private void TickCooldown()
        {
            m_RemainingCooldown -= Time.deltaTime;

            if (m_RemainingCooldown <= 0f)
            {
                m_RemainingCooldown = 0f;
                ChangeState(SkillActivationState.Idle);

                if (m_SkillConfig.Trigger.AutoActiveOnReady)
                {
                    TryActivate();
                }
            }
        }

        #endregion

        #region Execution

        private void ExecuteSkill()
        {
            ResetCastingData();

            ForceActivateAllInternal();

            if (m_SkillConfig.Trigger.TriggerState == SkillTriggerState.OnCooldownDone)
            {
                EnterCooldown();
            }
            else
            {
                ChangeState(SkillActivationState.Idle);
            }
        }
        public void ForceActivateAll()
        {
            ForceActivateAllInternal();
        }

        private void ForceActivateAllInternal()
        {
            if (m_State == SkillActivationState.Silenced)
                return;

            foreach (AbilityDeliver deliver in m_Delivers)
            {
                ActivateInternal(deliver);
            }

            m_OnActivates?.Invoke(m_Context);
        }
        public void ForceActivate(AbilityConfig config)
        {
            if (m_State == SkillActivationState.Silenced)
                return;
            if (HasAbilityInternal(config.BaseInfo.Id, out  var ability))
            {
                ActivateInternal(ability);
            }
        }
        public void ForceActivate(int index)
        {
            if (m_State == SkillActivationState.Silenced)
                return;
            ActivateInternal(m_Delivers[index]);
        }
        private void ActivateInternal(AbilityDeliver abilityDeliver)
        {
            abilityDeliver.Activate();
            m_OnActivateIndividu?.Invoke(abilityDeliver.AbilityContext);
        }

        #endregion

        #region Deliver Spawn

        private void SpawnDelivers()
        {
            foreach (AbilityConfig ability in m_SkillConfig.AbilitySets)
            {
                AbilityDeliver deliver = Instantiate(ability.DeliverPrefab, m_DeliverSpawnPost, false);

                deliver.Init(ability, m_Context);
                m_Delivers.Add(deliver);
            }
        }

        #endregion
    }
}
