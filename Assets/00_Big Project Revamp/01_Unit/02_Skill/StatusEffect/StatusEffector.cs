using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class StatusEffector : MonoBehaviour, IUpdater
    {
        [SerializeField] private StatusEffectConfig m_Config;
        [SerializeField] private UnityEvent<StatusEffectContext> m_OnApplied;
        [SerializeField] private UnityEvent<int, int> m_OnStackUpdated;
        [SerializeField] private UnityEvent<float> m_OnDurationUpdated;
        [SerializeField] private UnityEvent<StatusEffectContext> m_OnDone;

        private StatusEffectContext m_Context;
        private float m_RemainingDuration;

        [SerializeField, MMReadOnly]
        private int m_CurrentStack;

        private bool m_IsActive;

        public bool IsActive => m_IsActive;
        public StatusEffectConfig Config => m_Config;
        public int CurrentStack => m_CurrentStack;

        public void Initialize(StatusEffectConfig config)
        {
            m_Config = config;
            ResetRuntime();
        }
        private void OnEnable()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }
        private void OnDisable()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        private void ResetRuntime()
        {
            m_Context = null;
            m_CurrentStack = 0;
            m_RemainingDuration = 0f;
            m_IsActive = false;
        }

        public void ApplyEffect(StatusEffectConfig config, IAbilityContext context, Unit infected)
        {
            if (m_Config == null)
                m_Config = config;

            m_Context = new StatusEffectContext(context, infected);

            if (!m_IsActive)
            {
                ActivateFirstTime();
            }
            else
            {
                Reapply();
            }

            EvaluateRemovalCondition();
            m_OnApplied?.Invoke(m_Context);
        }

        private void ActivateFirstTime()
        {
            m_IsActive = true;
            m_CurrentStack = m_Config.InitialStack;
            m_RemainingDuration = m_Config.Duration;

            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);

            InvokeStackUpdated();
            InvokeDurationUpdated();

            m_Config.ApplyEffect(m_Context);

            for (int i = 1; i < m_CurrentStack; i++)
                m_Config.OnStackAdded(m_Context);
        }

        private void Reapply()
        {
            switch (m_Config.ReapplyBehavior)
            {
                case StatusReapplyBehavior.Stack:
                    ReapplyAsStack();
                    break;

                case StatusReapplyBehavior.Refresh:
                    ReapplyAsRefresh();
                    break;

                case StatusReapplyBehavior.Override:
                    ReapplyAsOverride();
                    break;
            }
        }

        private void ReapplyAsStack()
        {
            int previousStack = m_CurrentStack;
            m_CurrentStack = Mathf.Clamp(
                m_CurrentStack + m_Config.StackPerApply,
                0,
                m_Config.MaxStackCount);

            ApplyDurationRulesOnReapply();

            int delta = m_CurrentStack - previousStack;
            if (delta > 0)
            {
                for (int i = 0; i < delta; i++)
                    m_Config.OnStackAdded(m_Context);
            }
            else if (previousStack == m_Config.MaxStackCount)
            {
                m_Config.OnStackAdded(m_Context);
            }

            InvokeStackUpdated();
            InvokeDurationUpdated();
        }

        private void ReapplyAsRefresh()
        {
            ApplyDurationRulesOnReapply(forceReset: true);

            InvokeStackUpdated();
            InvokeDurationUpdated();
        }

        private void ReapplyAsOverride()
        {
            int previousStack = m_CurrentStack;

            if (previousStack > 0)
            {
                for (int i = 0; i < previousStack; i++)
                    m_Config.OnStackRemoved(m_Context);
            }

            m_CurrentStack = m_Config.InitialStack;
            m_RemainingDuration = m_Config.Duration;

            m_Config.ApplyEffect(m_Context);

            for (int i = 1; i < m_CurrentStack; i++)
                m_Config.OnStackAdded(m_Context);

            InvokeStackUpdated();
            InvokeDurationUpdated();
        }

        private void ApplyDurationRulesOnReapply(bool forceReset = false)
        {
            if (forceReset || m_Config.ResetDurationOnReapply)
            {
                m_RemainingDuration = m_Config.Duration;
                return;
            }

            if (m_Config.UseBonusDurationPerReapply)
            {
                m_RemainingDuration += m_Config.BonusDurationPerReapply;
            }
        }

        public void Tick()
        {
            if (!m_IsActive)
                return;

            if (m_Config.HowToRemove != HowStatRemoved.RemoveOnDurationEnd)
                return;

            m_RemainingDuration -= Time.deltaTime;
            InvokeDurationUpdated();

            if (m_RemainingDuration <= 0f)
                RemoveOneStack();
        }

        public void RemoveEffect()
        {
            if (!m_IsActive)
                return;

            RemoveOneStack();
        }

        private void RemoveOneStack()
        {
            int previousStack = m_CurrentStack;
            if (previousStack <= 0)
                return;

            m_CurrentStack = Mathf.Max(0, m_CurrentStack - 1);

            int delta = previousStack - m_CurrentStack;
            for (int i = 0; i < delta; i++)
                m_Config.OnStackRemoved(m_Context);

            if (m_CurrentStack > 0)
            {
                m_RemainingDuration = m_Config.Duration;
            }

            InvokeStackUpdated();
            InvokeDurationUpdated();

            EvaluateRemovalCondition();
        }

        private void EvaluateRemovalCondition()
        {
            switch (m_Config.HowToRemove)
            {
                case HowStatRemoved.None:
                    return;

                case HowStatRemoved.RemoveOnDurationEnd:
                    if (m_CurrentStack <= 0)
                        CompleteEffect();
                    break;

                case HowStatRemoved.RemoveOnStackZero:
                    if (m_CurrentStack <= 0)
                        CompleteEffect();
                    break;

                case HowStatRemoved.RemoveOnStackExceedMax:
                    if (m_CurrentStack >= m_Config.MaxStackCount)
                        CompleteEffect();
                    break;
            }
        }

        private void CompleteEffect()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);

            m_Config.DoneEffect(m_Context);
            m_OnDone?.Invoke(m_Context);

            ResetRuntime();
            gameObject.SetActive(false);
        }

        private void InvokeStackUpdated()
        {
            if (m_Config == null)
                return;

            m_OnStackUpdated?.Invoke(m_CurrentStack, m_Config.MaxStackCount);
        }

        private void InvokeDurationUpdated()
        {
            if (m_Config == null || m_Config.Duration <= 0f)
                return;

            float normalized = Mathf.Clamp01(m_RemainingDuration / m_Config.Duration);
            m_OnDurationUpdated?.Invoke(normalized);
        }
    }
}