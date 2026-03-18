using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class StatusEffector : MonoBehaviour, IUpdater
    {
        [SerializeField] private StatusEffectConfig m_Config;
        [SerializeField] private UnityEvent<StatusEffectContext> m_OnApplied;
        [SerializeField] private UnityEvent<int, int> m_OnStackUpdated; // current stack, max stack
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

        private void ResetRuntime()
        {
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
                Activate();
            }

            UpdateStackOnApply();
            UpdateDurationOnApply();
            EvaluateRemovalCondition();

            m_OnApplied?.Invoke(m_Context);
        }

        private void Activate()
        {
            m_IsActive = true;
            m_CurrentStack = m_Config.GetStartingStack();
            m_RemainingDuration = m_Config.Duration;

            InvokeStackUpdated();

            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
            RecalculateEffect(0, m_CurrentStack);
        }

        private void UpdateStackOnApply()
        {
            int previousStack = m_CurrentStack;
            int stackBeforeClamp = m_CurrentStack;

            switch (m_Config.HowStackUpdate)
            {
                case HowStackUpdate.Addictive:
                    stackBeforeClamp += m_Config.UpdatePerStackCount;
                    break;

                case HowStackUpdate.Subtractive:
                    stackBeforeClamp -= m_Config.UpdatePerStackCount;
                    break;
            }

            m_CurrentStack = Mathf.Clamp(stackBeforeClamp, 0, m_Config.MaxStackCount);

            InvokeStackUpdated();

            // Edge case:
            // if already at max stack and trying to add more stack,
            // still trigger OnStackAdded as a "max stack re-apply" behavior.
            if (m_CurrentStack == m_Config.MaxStackCount &&
                previousStack == m_Config.MaxStackCount &&
                m_Config.HowStackUpdate == HowStackUpdate.Addictive)
            {
                m_Config.OnStackAdded(m_Context);
                return;
            }

            RecalculateEffect(previousStack, m_CurrentStack);
        }

        private void RecalculateEffect(int oldStack, int newStack)
        {
            if (oldStack == newStack)
                return;

            if (oldStack == 0 && newStack > 0)
            {
                m_Config.ApplyEffect(m_Context);

                for (int i = 1; i < newStack; i++)
                    m_Config.OnStackAdded(m_Context);

                return;
            }

            if (newStack > oldStack)
            {
                int delta = newStack - oldStack;

                for (int i = 0; i < delta; i++)
                    m_Config.OnStackAdded(m_Context);

                return;
            }

            if (newStack < oldStack)
            {
                int delta = oldStack - newStack;

                for (int i = 0; i < delta; i++)
                    m_Config.OnStackRemoved(m_Context);
            }
        }

        private void UpdateDurationOnApply()
        {
            if (m_Config.ResetDurationOnStackUpdate)
            {
                m_RemainingDuration = m_Config.Duration;
                return;
            }

            if (m_Config.UseStackDuration)
            {
                m_RemainingDuration += m_Config.StackDurationUpdate;
            }
        }

        public void Tick()
        {
            if (!m_IsActive)
                return;

            if (m_Config.HowToRemove != HowStatRemoved.RemoveOnDurationEnd)
                return;

            m_RemainingDuration -= Time.deltaTime;

            m_OnDurationUpdated?.Invoke(
                Mathf.Clamp01(m_RemainingDuration / m_Config.Duration));

            if (m_RemainingDuration <= 0f)
            {
                RemoveOneStack();
            }
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

            m_CurrentStack--;
            m_CurrentStack = Mathf.Max(0, m_CurrentStack);

            InvokeStackUpdated();

            RecalculateEffect(previousStack, m_CurrentStack);
            EvaluateRemovalCondition();
        }

        private void EvaluateRemovalCondition()
        {
            switch (m_Config.HowToRemove)
            {
                case HowStatRemoved.None:
                    return;

                case HowStatRemoved.RemoveOnDurationEnd:
                    if (m_RemainingDuration <= 0f && m_CurrentStack <= 0)
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
    }
}