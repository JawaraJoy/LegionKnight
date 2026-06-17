using LegionKnight;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    
    public partial class Skill : Bindable, IUpdater, ISkill, IReseter
    {
        [Header("Config")]
        [SerializeField]
        private SkillConfig m_SkillConfig;
        [SerializeField]
        private ProgressField m_Progression;
        [SerializeField]
        private Transform m_DeliverSpawnPost;

        [Header("Runtime")]
        [SerializeField, MMReadOnly]
        private SkillContext m_SkillContext;
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
        private UnityEvent<IAbilityContext> m_OnActivateIndividu;
        [SerializeField]
        private UnityEvent<Skill> m_OnTryActivate;
        [SerializeField]
        private UnityEvent<Skill> m_OnActivates;
        [SerializeField]
        private UnityEvent<int, int> m_OnChargeUpdate;
        [SerializeField]
        private UnityEvent<Unit> m_OnAbilityDelivered;
        public ProgressField Progression => m_Progression;
        public SkillConfig SkillConfig => m_SkillConfig;
        public SkillContext SkillContext => m_SkillContext;
        public IReadOnlyList<AbilityDeliver> Delivers => m_Delivers;
        public UnityEvent<Unit> OnAbilityDelivered => m_OnAbilityDelivered;
        public UnityEvent OnCastingStartEvent => m_OnCastingStart;
        public UnityEvent<float> OnCastingUpdateEvent => m_OnCastingDurationUpdate;
        public UnityEvent OnCastingSuccessEvent => m_OnCastingSuccess;
        public UnityEvent OnCastingFailEvent => m_OnCastingFail;

        public bool IsActive => !CardPanel.IsShown || !PausePanel.IsShown;

        private RogueLikeCardPanel m_CardPanel;
        private RogueLikeCardPanel CardPanel
        {
            get
            {
                if (m_CardPanel == null)
                {
                    m_CardPanel = CanvasManager.Instance.GetPanel<RogueLikeCardPanel>();
                }
                return m_CardPanel;
            }
        }
        private PausePanel m_PausePanel;
        private PausePanel PausePanel
        {
            get
            {
                if (m_PausePanel == null)
                {
                    m_PausePanel = CanvasManager.Instance.GetPanel<PausePanel>();
                }
                return m_PausePanel;
            }
        }

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
        private int m_CurrentInterruptDamage;
        [SerializeField, MMReadOnly]
        private int m_MaxInterruptDamage;

        public float RemainingCastTime => m_RemainingCastTime;
        public int CurrentInterruptCount => m_CurrentInterruptDamage;
        public int MaxInterruptCount => m_MaxInterruptDamage;

        public UnityEvent<Skill> OnTryActivate => m_OnTryActivate;
        public UnityEvent<Skill> OnActivate => m_OnActivates;
        public UnityEvent<int, int> OnChargeUpdate => m_OnChargeUpdate;
        public UnityEvent<int, int> OnCastingInterruptEvent => m_OnCastingInterruptUpdate;

        public float RemainingCharge => m_RemainingCharge;
        private AbilityDeliver GetAbilityDeliverInternal(string id)
        {
            return m_Delivers.Find(x => x.AbilityConfig.BaseInfo.Id == id);
        }

        private bool HasAbilityInternal(string id, out AbilityDeliver abilityDeliver)
        {
            abilityDeliver = GetAbilityDeliverInternal(id);
            return abilityDeliver != null;
        }
        public bool HasAbility(string id, out AbilityDeliver abilityDeliver)
        {
            return HasAbilityInternal(id, out abilityDeliver);
        }

        #region Unity Lifecycle

        private void Start()
        {
            CardPanel.OnShow.AddListener(UnregisterCasting);
            CardPanel.OnHide.AddListener(RegisterCasting);

            PausePanel.OnShow.AddListener(UnregisterCasting);
            PausePanel.OnHide.AddListener(RegisterCasting);
        }

        private void RegisterCasting()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }
        private void UnregisterCasting()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        #endregion

        #region Init

        private int calledInit;
        public virtual void Init(SkillConfig skillConfig, IModuleContext ownerContext)
        {
            m_SkillConfig = skillConfig;
            m_SkillContext = new SkillContext(this, ownerContext);

            m_OnInit?.Invoke(m_SkillContext);
            m_Progression.SetLevel(skillConfig.LevelSet.Level);
            m_Progression.SetMaxLevel(skillConfig.LevelSet.MaxLevel);
            ChangeState(m_SkillConfig.InitializeState);
            SpawnDelivers();
            Debug.Log($"Skill {m_SkillConfig.BaseInfo.Id} initialized with state {m_State}. Called Init {++calledInit} times.");
        }

        #endregion

        #region Update

        public void Tick()
        {
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

        

        #endregion

        #region Charge

        private void SetChargeInternal(float amount)
        {
            m_RemainingCharge = amount;
            OnChargeUpdateInvoke(m_RemainingCharge, m_SkillConfig.Activation.Charge);
        }

        public void AddCharge(float amount)
        {
            AddChargeInternal(amount);
        }

        private void AddChargeInternal(float amount)
        {
            if (m_State == SkillActivationState.Silenced)
                return;

            m_RemainingCharge += amount;

            if (m_RemainingCharge >= m_SkillConfig.Activation.Charge)
            {
                float overflow = m_RemainingCharge - m_SkillConfig.Activation.Charge;

                if (m_SkillConfig.Activation.ForceActiveState == ForceActiveState.OnChargeFull &&
                    m_SkillConfig.Activation.AutoActiveOnReady)
                {
                    TryActivateInternal();
                }

                m_RemainingCharge = Mathf.Max(0f, overflow);
            }
            OnChargeUpdateInvoke(m_RemainingCharge, m_SkillConfig.Activation.Charge);
        }

        private void OnChargeUpdateInvoke(float current, float max)
        {
            int roundedCurrent = Mathf.RoundToInt(current);
            int roundedMax = Mathf.RoundToInt(max);
            m_OnChargeUpdate?.Invoke(roundedCurrent, roundedMax);
        }
        public void AddAbility(AbilityConfig abilityConfig)
        {
            SpawnDeliver(abilityConfig);
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

            RegisterCasting();
            m_OnTryActivate?.Invoke(this);
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
            m_CurrentInterruptDamage = 0;

            Unit unit = m_SkillContext.ModuleContext.Unit;
            if (unit.HasBind(out Damageable damageable))
            {
                int finalHealth = damageable.MaxHealth;
                float rate = m_SkillConfig.Casting.MaxInteruptResist;
                m_MaxInterruptDamage = Mathf.RoundToInt(finalHealth * rate);
            }
            

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

        public void TakeInteruptDamage(int amount)
        {
            if (m_State != SkillActivationState.Casting)
                return;

            m_CurrentInterruptDamage += amount;

            m_OnCastingInterruptUpdate?.Invoke(m_CurrentInterruptDamage, m_MaxInterruptDamage);

            if (m_CurrentInterruptDamage >= m_MaxInterruptDamage)
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
            m_CurrentInterruptDamage = 0;
            m_MaxInterruptDamage = 0;
        }

        #endregion

        #region Cooldown

        private void EnterCooldown()
        {
            m_RemainingCooldown = m_SkillConfig.Activation.Cooldown;
            ChangeState(SkillActivationState.Cooldown);
        }

        private void TickCooldown()
        {
            m_RemainingCooldown -= Time.deltaTime;

            if (m_RemainingCooldown <= 0f)
            {
                m_RemainingCooldown = 0f;
                ChangeState(SkillActivationState.Idle);

                if (m_SkillConfig.Activation.AutoActiveOnReady)
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

            if (m_SkillConfig.Activation.ForceActiveState == ForceActiveState.OnCooldownDone)
            {
                EnterCooldown();
            }
            else
            {
                ChangeState(SkillActivationState.Idle);
            }
            UnregisterCasting();
        }
        public void ForceActivateAll()
        {
            ForceActivateAllInternal();
        }

        private void ForceActivateAllInternal()
        {
            if (m_State == SkillActivationState.Silenced)
                return;

            float delay = m_SkillConfig.Activation.IntervalEachAbilityActive;
            if (gameObject.activeInHierarchy)
                RushGameManager.Instance.StartCoroutine(ForceActivateAllWithDelay(delay));
        }
        private IEnumerator ForceActivateAllWithDelay(float delay)
        {
            for (int i = 0; i < m_Delivers.Count; i++)
            {
                ForceActivateInternal(m_Delivers[i].AbilityConfig);
                yield return new WaitForSeconds(delay);
            }
            m_OnActivates?.Invoke(this);
        }
        public void ForceActivate(AbilityConfig config)
        {
            ForceActivateInternal(config);
        }
        private void ForceActivateInternal(AbilityConfig config)
        {
            if (m_State == SkillActivationState.Silenced)
                return;
            if (HasAbilityInternal(config.BaseInfo.Id, out var ability))
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
                SpawnDeliver(ability);
            }
        }

        private void SpawnDeliver(AbilityConfig abilityConfig)
        {
            AbilityDeliver deliver = Instantiate(abilityConfig.DeliverPrefab, m_DeliverSpawnPost, false);
            deliver.Init(abilityConfig, m_SkillContext);
            m_Delivers.Add(deliver);
        }

        private void ClearNonSkillConfigDelivers()
        {
            for (int i = m_Delivers.Count - 1; i >= 0; i--)
            {
                if (!HasAbilityInternal(m_Delivers[i].AbilityConfig.BaseInfo.Id, out AbilityDeliver abilityDeliver))
                {
                    Destroy(abilityDeliver.gameObject);
                    m_Delivers.RemoveAt(i);
                }
            }

        }

        public void ResetProgression()
        {
            m_Progression.SetLevel(1);
            SetChargeInternal(0);
        }

        #endregion
    }
}
