
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class StatModifier : MonoBehaviour, IUpdater
    {
        private float m_TotalDuration = 0f;
        private float m_RemainingDuration = 1f;
        private float m_TotalStackUpdateDuration = 10f;
        private float m_RemainingStackUpdateDuration = 0f;
        private int m_StackCount = 0;
        private StatModifierContext m_Context;
        [SerializeField]
        private UnityEvent<StatModifierContext> m_OnActive;
        [SerializeField]
        private UnityEvent<StatModifierContext> m_OnDeactive;
        [SerializeField]
        private UnityEvent<int> m_OnStackChange;
        [SerializeField]
        private UnityEvent<float> m_OnDurationUpdate;
        [SerializeField]
        private UnityEvent m_OnDurationEnd;
        [SerializeField]
        private UnityEvent m_OnStackEmpty;
        [SerializeField]
        private UnityEvent m_OnStackExceedMax;
        public float RemainingDuration => m_RemainingDuration;
        private float TotalStackUpdateDurationInternal
        {
            get
            {
                return Mathf.Clamp(m_TotalStackUpdateDuration, 0f, m_TotalDuration);
            }
        }
        public int StackCount => m_StackCount;
        public StatModifierContext Context => m_Context;
        public bool IsActive => gameObject.activeInHierarchy;

        private StatModifierConfig m_Config;
        public StatModifierConfig Config => m_Config;

        private StatController m_TargetController;
        public StatController TargetController => m_TargetController;
        public void Activate(AbilityContext context, StatController targetController)
        {
            ClearListeners();
            gameObject.SetActive(true);
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);

            m_Context = new StatModifierContext(context, this);
            m_TargetController = targetController;
            AbilityConfig abilityConfig = context.AbilityDeliver.AbilityConfig;
            if (abilityConfig is StatModifierConfig config)
            {
                m_Config = config;
            }

            int skillLevel = m_Context.AbilityContext.SkillContext.Skill.Progression.Level;
            m_TotalDuration = m_Config.FinalDurationByLevel(skillLevel);
            SetStackInternal(m_Config.GetStartingStack());

            switch (m_Config.HowToRemove)
            {
                case HowStatRemoved.None:
                    break;
                case HowStatRemoved.RemoveOnDurationEnd:
                    //m_OnDurationEnd.RemoveListener(OnDeactiveInvokeInternal);
                    m_OnDurationEnd.AddListener(OnDeactiveInvokeInternal);
                    break;
                case HowStatRemoved.RemoveOnStackZero:
                    //m_OnStackEmpty.RemoveListener(OnDeactiveInvokeInternal);
                    m_OnStackEmpty.AddListener(OnDeactiveInvokeInternal);
                    break;
                case HowStatRemoved.RemoveOnStackExceedMax:
                    //m_OnStackExceedMax.RemoveListener(OnDeactiveInvokeInternal);
                    m_OnStackExceedMax.AddListener(OnDeactiveInvokeInternal);
                    break;
            }
            StartTimer();
            m_RemainingStackUpdateDuration = TotalStackUpdateDurationInternal;
            m_OnActive?.Invoke(m_Context);
            RefreshDamageStat(m_TargetController);
        }
        public void UpdateStack()
        {
            switch (m_Config.HowStackUpdate)
            {
                case HowStackUpdate.Addictive:
                    AddStackInternal(m_Config.UpdatePerStackCount);
                    break;
                case HowStackUpdate.Subtractive:
                    AddStackInternal(-m_Config.UpdatePerStackCount);
                    break;
            }
        }

        public void OnDeactiveInvoke()
        {
            OnDeactiveInvokeInternal();
        }
        private void OnDeactiveInvokeInternal()
        {
            ClearListeners();

            m_OnDeactive?.Invoke(m_Context);
            gameObject.SetActive(false);
            m_StackCount = 0;
            m_RemainingDuration = 0;
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);

            if (m_TargetController.ModuleContext.Unit.HasBind(out SkillController skillController))
            {
                skillController.ForceActives(m_Config.InfluencedSkillToActivateOnStackEmpty);
            }
        }
        private void SetStackInternal(int stack)
        {
            m_StackCount = stack;
            OnStackUpdateInvoke();
        }
        private void AddStackInternal(int add)
        {
            m_StackCount += add;
            OnStackUpdateInvoke();
        }

        private void RefreshDamageStat(StatController targetController)
        {
            Unit unit = targetController.ModuleContext.Unit;
            if (unit.HasBind(out Damageable damageable))
            {
                damageable.RefreshDamageableStat(1f, false);
            }
        }
        private void OnStackUpdateInvoke()
        {
            m_OnStackChange?.Invoke(m_StackCount);
            
            if (m_StackCount >= m_Config.MaxStackCount)
            {
                if (m_TargetController.ModuleContext.Unit.HasBind(out SkillController skillController))
                {
                    skillController.ForceActives(m_Config.InfluencedSkillToActivateOnStackFull);
                }
            }
            m_RemainingStackUpdateDuration = TotalStackUpdateDurationInternal;
            if (m_Config.ResetDurationOnStackUpdate)
            {
                StartTimer();
            }
            if (m_Config.HowToRemove == HowStatRemoved.RemoveOnStackExceedMax)
            {
                if (m_StackCount > m_Config.MaxStackCount)
                {
                    OnDeactiveInvokeInternal();
                    m_OnStackExceedMax?.Invoke();
                }
            }
            if (m_Config.HowToRemove == HowStatRemoved.RemoveOnStackZero)
            {
                if (m_StackCount <= 0)
                {
                    OnDeactiveInvokeInternal();
                    m_OnStackEmpty?.Invoke();
                }
            }
            RefreshDamageStat(m_TargetController);

            m_StackCount = Mathf.Clamp(m_StackCount, 0, m_Config.MaxStackCount);
        }
        private void ClearListeners()
        {
            m_OnDurationEnd.RemoveAllListeners();
            m_OnStackEmpty.RemoveAllListeners();
            m_OnStackExceedMax.RemoveAllListeners();
        }

        /// <summary>
        /// Initializes or resets the timer back to initial duration.
        /// </summary>
        public void ResetTimer()
        {
            m_RemainingDuration = m_TotalDuration;
        }
        private void StartTimer()
        {
            int level = m_Context.AbilityContext.SkillContext.Skill.Progression.Level;
            m_TotalDuration = m_Config.FinalDurationByLevel(level);
            m_RemainingDuration = m_TotalDuration;
        }
        /// <summary>
        /// Ticks the timer. Call this every frame (Update or custom tick system).
        /// </summary>
        public void Tick()
        {
            m_RemainingDuration -= Time.deltaTime;
            float normalized = m_TotalDuration > 0f ? m_RemainingDuration / m_TotalDuration : 0f;
            m_OnDurationUpdate?.Invoke(normalized);

            if (m_RemainingDuration <= 0f)
            {
                m_RemainingDuration = 0f;
                m_OnDurationEnd?.Invoke();
            }
            if (m_Config.UseStackDuration) 
            {
                TickStackDuration();
            }
        }

        private void TickStackDuration()
        {
            m_RemainingStackUpdateDuration -= Time.deltaTime;

            if (m_RemainingStackUpdateDuration <= 0f)
            {
                AddStackInternal(-m_Config.UpdatePerStackCount);
                m_RemainingStackUpdateDuration = TotalStackUpdateDurationInternal;
            }
        }

        /// <summary>
        /// Returns true if the timer has finished.
        /// </summary>
        public bool IsFinished()
        {
            return m_RemainingDuration <= 0f;
        }

        /// <summary>
        /// Returns remaining time in seconds.
        /// </summary>
        public float GetRemainingDuration()
        {
            return m_RemainingDuration;
        }
    }
}
