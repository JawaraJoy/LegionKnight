using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class StatusEffector : MonoBehaviour, IUpdater
    {
        [Header("Config")]
        [SerializeField] private StatusEffectConfig m_Config;

        [Header("Events")]
        [SerializeField] private UnityEvent<StatusEffectContext> m_OnApplied;
        [SerializeField] private UnityEvent<int, int> m_OnStackUpdated;
        [SerializeField] private UnityEvent<float> m_OnMainDurationUpdated;
        [SerializeField] private UnityEvent<float> m_OnStackDurationUpdated;
        [SerializeField] private UnityEvent<StatusEffectContext> m_OnDone;

        [Header("Runtime")]
        [SerializeField, MMReadOnly] private int m_CurrentStack;
        [SerializeField, MMReadOnly] private float m_RemainingMainDuration;
        [SerializeField, MMReadOnly] private float m_RemainingStackDuration;
        [SerializeField, MMReadOnly] private bool m_IsActive;

        private StatusEffectContext m_Context;

        public bool IsActive => m_IsActive;
        public StatusEffectConfig Config => m_Config;
        public int CurrentStack => m_CurrentStack;
        public float RemainingMainDuration => m_RemainingMainDuration;
        public float RemainingStackDuration => m_RemainingStackDuration;

        public void Initialize(StatusEffectConfig config)
        {
            m_Config = config;
            ResetRuntime();
        }

        private void OnDisable()
        {
            UnregisterTick();
        }

        private void ResetRuntime()
        {
            m_Context = null;
            m_CurrentStack = 0;
            m_RemainingMainDuration = 0f;
            m_RemainingStackDuration = 0f;
            m_IsActive = false;
        }

        public void ApplyEffect(StatusEffectConfig config, IAbilityContext context, Unit infected)
        {
            if (config == null)
            {
                Debug.LogError($"{name} -> ApplyEffect failed: config is null.");
                return;
            }

            if (m_Config == null)
            {
                m_Config = config;
            }
            else if (m_Config != config)
            {
                Debug.LogError($"{name} -> StatusEffector received a different config than the initialized one.");
                return;
            }

            m_Context = new StatusEffectContext(context, infected);

            if (!m_IsActive)
            {
                ActivateFirstTime();
            }
            else
            {
                Reapply();
            }

            if (m_IsActive)
                m_OnApplied?.Invoke(m_Context);
        }

        private void ActivateFirstTime()
        {
            m_IsActive = true;

            m_Config.OnEffectStarted(m_Context);

            m_CurrentStack = 0;
            AddStacks(m_Config.InitialStack);

            ResetMainDuration();
            ResetStackDecayTimerClamped();

            RegisterTick();
            InvokeAllRuntimeEvents();

            EvaluateEndConditions();
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
            int attemptedStack = m_CurrentStack + m_Config.StackPerApply;
            bool reachedMax = attemptedStack >= m_Config.MaxStackCount;
            bool exceededMax = attemptedStack > m_Config.MaxStackCount;

            AddStacks(m_Config.StackPerApply);

            if (m_Config.UseMainDuration && m_Config.ResetMainDurationOnReapply)
                ResetMainDuration();

            if (m_Config.UseStackDecay)
            {
                if (m_Config.ResetStackDecayTimerOnReapply)
                    ResetStackDecayTimerClamped();
                else
                    ClampStackDecayTimerToMainDuration();
            }

            InvokeAllRuntimeEvents();

            EvaluateSpecialRemovalRules(reachedMax, exceededMax);
            EvaluateEndConditions();
        }

        private void ReapplyAsRefresh()
        {
            if (m_Config.UseMainDuration && m_Config.ResetMainDurationOnReapply)
                ResetMainDuration();

            if (m_Config.UseStackDecay)
            {
                if (m_Config.ResetStackDecayTimerOnReapply)
                    ResetStackDecayTimerClamped();
                else
                    ClampStackDecayTimerToMainDuration();
            }

            InvokeAllRuntimeEvents();
            EvaluateEndConditions();
        }

        private void ReapplyAsOverride()
        {
            ForceEndEffect(false);

            m_IsActive = true;
            m_Config.OnEffectStarted(m_Context);

            m_CurrentStack = 0;
            AddStacks(m_Config.InitialStack);

            ResetMainDuration();
            ResetStackDecayTimerClamped();

            RegisterTick();
            InvokeAllRuntimeEvents();

            EvaluateEndConditions();
        }

        public void Tick()
        {
            if (!m_IsActive || m_Config == null)
                return;

            float deltaTime = Time.deltaTime;

            TickMainDuration(deltaTime);

            if (!m_IsActive)
                return;

            TickStackDecay(deltaTime);

            if (!m_IsActive)
                return;

            ClampStackDecayTimerToMainDuration();
            InvokeAllRuntimeEvents();
            EvaluateEndConditions();
        }

        private void TickMainDuration(float deltaTime)
        {
            if (!m_Config.UseMainDuration)
                return;

            m_RemainingMainDuration -= deltaTime;

            if (m_RemainingMainDuration <= 0f)
                m_RemainingMainDuration = 0f;

            if (m_Config.RemoveRule == StatusRemoveRule.OnMainDurationEnd &&
                m_RemainingMainDuration <= 0f)
            {
                CompleteEffect();
            }
        }

        private void TickStackDecay(float deltaTime)
        {
            if (!m_Config.UseStackDecay)
                return;

            if (m_CurrentStack <= 0)
                return;

            m_RemainingStackDuration -= deltaTime;

            if (m_RemainingStackDuration <= 0f)
            {
                RemoveStacksByDecay();
            }
        }

        /// <summary>
        /// Menghapus seluruh effect secara paksa.
        /// </summary>
        public void RemoveEffect()
        {
            if (!m_IsActive)
                return;

            CompleteEffect();
        }

        /// <summary>
        /// Menghapus stack secara manual.
        /// </summary>
        public void RemoveStacksManually(int amount)
        {
            if (!m_IsActive)
                return;

            RemoveStacks(amount);

            if (!m_IsActive)
                return;

            if (m_CurrentStack > 0 && m_Config.UseStackDecay)
                ResetStackDecayTimerClamped();

            InvokeAllRuntimeEvents();
            EvaluateEndConditions();
        }

        private void RemoveStacksByDecay()
        {
            RemoveStacks(m_Config.StackDecayAmountPerInterval);

            if (!m_IsActive)
                return;

            if (m_CurrentStack > 0)
            {
                if (m_Config.UseMainDuration && m_Config.ResetMainDurationOnStackDecay)
                    ResetMainDuration();

                if (m_Config.UseStackDecay)
                    ResetStackDecayTimerClamped();
            }
            else
            {
                m_RemainingStackDuration = 0f;
            }

            InvokeAllRuntimeEvents();
            EvaluateEndConditions();
        }

        private void AddStacks(int amount)
        {
            if (amount <= 0)
                return;

            int targetStack = Mathf.Clamp(m_CurrentStack + amount, 0, m_Config.MaxStackCount);
            int delta = targetStack - m_CurrentStack;

            for (int i = 0; i < delta; i++)
            {
                m_CurrentStack++;
                m_Config.OnStackAdded(m_Context);
            }
        }

        private void RemoveStacks(int amount)
        {
            if (amount <= 0)
                return;

            int removable = Mathf.Min(amount, m_CurrentStack);

            for (int i = 0; i < removable; i++)
            {
                m_CurrentStack--;
                m_Config.OnStackRemoved(m_Context);
            }
        }

        private void ResetMainDuration()
        {
            if (!m_Config.UseMainDuration)
            {
                m_RemainingMainDuration = 0f;
                return;
            }

            m_RemainingMainDuration = m_Config.MainDuration;
        }

        private void ResetStackDecayTimerClamped()
        {
            if (!m_Config.UseStackDecay)
            {
                m_RemainingStackDuration = 0f;
                return;
            }

            float newDuration = m_Config.StackDecayInterval;

            if (m_Config.UseMainDuration)
                newDuration = Mathf.Min(newDuration, m_RemainingMainDuration);

            m_RemainingStackDuration = Mathf.Max(0f, newDuration);
        }

        private void ClampStackDecayTimerToMainDuration()
        {
            if (!m_Config.UseStackDecay)
                return;

            if (!m_Config.UseMainDuration)
                return;

            if (m_RemainingStackDuration > m_RemainingMainDuration)
                m_RemainingStackDuration = m_RemainingMainDuration;
        }

        private void EvaluateSpecialRemovalRules(bool reachedMax, bool exceededMax)
        {
            if (!m_IsActive)
                return;

            switch (m_Config.RemoveRule)
            {
                case StatusRemoveRule.OnStackReachMax:
                    if (reachedMax && m_CurrentStack >= m_Config.MaxStackCount)
                    {
                        CompleteEffect();
                    }
                    break;

                case StatusRemoveRule.OnStackExceedMax:
                    if (exceededMax)
                    {
                        CompleteEffect();
                    }
                    break;
            }
        }

        private void EvaluateEndConditions()
        {
            if (!m_IsActive)
                return;

            switch (m_Config.RemoveRule)
            {
                case StatusRemoveRule.None:
                    break;

                case StatusRemoveRule.OnMainDurationEnd:
                    if (m_Config.UseMainDuration && m_RemainingMainDuration <= 0f)
                        CompleteEffect();
                    break;

                case StatusRemoveRule.OnStackZero:
                    if (m_CurrentStack <= 0)
                        CompleteEffect();
                    break;

                case StatusRemoveRule.OnStackReachMax:
                    if (m_CurrentStack >= m_Config.MaxStackCount)
                        CompleteEffect();
                    break;

                case StatusRemoveRule.OnStackExceedMax:
                    // dicek khusus saat reapply/add stack
                    break;
            }
        }

        private void CompleteEffect()
        {
            if (!m_IsActive)
                return;

            ForceEndEffect(true);
        }

        private void ForceEndEffect(bool deactivateGameObject)
        {
            UnregisterTick();

            StatusEffectContext cachedContext = m_Context;

            if (m_Config != null && cachedContext != null)
                m_Config.OnEffectEnded(cachedContext);

            m_OnDone?.Invoke(cachedContext);

            ResetRuntime();

            if (deactivateGameObject)
                gameObject.SetActive(false);
        }

        private void RegisterTick()
        {
            if (UpdateBank.Instance != null)
                UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }

        private void UnregisterTick()
        {
            if (UpdateBank.Instance != null)
                UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        private void InvokeAllRuntimeEvents()
        {
            InvokeStackUpdated();
            InvokeMainDurationUpdated();
            InvokeStackDurationUpdated();
        }

        private void InvokeStackUpdated()
        {
            if (m_Config == null)
                return;

            m_OnStackUpdated?.Invoke(m_CurrentStack, m_Config.MaxStackCount);
        }

        private void InvokeMainDurationUpdated()
        {
            if (m_Config == null || !m_Config.UseMainDuration)
                return;

            if (m_Config.MainDuration <= 0f)
            {
                m_OnMainDurationUpdated?.Invoke(0f);
                return;
            }

            float normalized = Mathf.Clamp01(m_RemainingMainDuration / m_Config.MainDuration);
            m_OnMainDurationUpdated?.Invoke(normalized);
        }

        private void InvokeStackDurationUpdated()
        {
            if (m_Config == null || !m_Config.UseStackDecay)
                return;

            float maxDuration = m_Config.StackDecayInterval;

            if (m_Config.UseMainDuration)
                maxDuration = Mathf.Min(maxDuration, m_Config.MainDuration);

            maxDuration = Mathf.Max(0.01f, maxDuration);

            float normalized = Mathf.Clamp01(m_RemainingStackDuration / maxDuration);
            m_OnStackDurationUpdated?.Invoke(normalized);
        }
    }
}