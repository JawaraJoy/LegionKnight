using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class StatusEffector : MonoBehaviour, IUpdater
    {
        [SerializeField] 
        private StatusEffectConfig m_Config;
        [SerializeField] 
        private UnityEvent<StatusEffectContext> m_OnApplied;
        [SerializeField] 
        private UnityEvent<float> m_OnDurationUpdated;
        [SerializeField] 
        private UnityEvent<StatusEffectContext> m_OnDone;

        [SerializeField, MMReadOnly] 
        private Unit m_Target;
        [SerializeField, MMReadOnly] 
        private StatusEffectContext m_Context;
        [SerializeField, MMReadOnly] 
        private float m_RemainingDuration;
        [SerializeField, MMReadOnly] 
        private int m_CurrentStack;

        public StatusEffectConfig Config => m_Config;
        public Unit Target => m_Target;
        public StatusEffectContext Context => m_Context;
        public bool IsActive => m_CurrentStack > 0;

        #region INITIALIZE

        public void Initialize(StatusEffectConfig config)
        {
            m_Config = config;
            m_CurrentStack = 0;
            m_RemainingDuration = 0f;
        }

        #endregion

        #region APPLY

        public void ApplyEffect(StatusEffectConfig config, IAbilityContext abilityContext, Unit target)
        {
            if (m_Config == null)
                m_Config = config;

            if (m_CurrentStack >= m_Config.MaxStack)
                return;

            m_Target = target;
            m_Context = new StatusEffectContext(abilityContext, this);

            AddStack();

            if (m_CurrentStack == 1)
            {
                UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
            }

            if (m_Config.ResetDurationWhenStacked || m_CurrentStack == 1)
            {
                m_RemainingDuration = m_Config.Duration;
            }

            m_OnApplied?.Invoke(m_Context);
            Debug.Log($"Applied {m_Config.name} to {target.name}. Current Stack: {m_CurrentStack}");
        }

        private void AddStack()
        {
            m_CurrentStack++;
            m_CurrentStack = Mathf.Clamp(m_CurrentStack, 0, m_Config.MaxStack);

            m_Config.ApplyEffect(m_Target);
        }

        #endregion

        #region REMOVE

        public void RemoveEffect()
        {
            if (m_CurrentStack <= 0)
                return;

            RemoveOneStack();
        }

        private void RemoveOneStack()
        {
            m_CurrentStack--;

            m_Config.OnStackRemoved(m_Target);

            if (m_CurrentStack <= 0)
            {
                CompleteEffect();
            }
            else
            {
                m_RemainingDuration = m_Config.Duration;
            }
        }

        private void CompleteEffect()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
            m_Config.DoneEffect(m_Target);
            m_OnDone?.Invoke(m_Context);
            gameObject.SetActive(false);
        }

        #endregion

        #region TICK

        public void Tick()
        {
            if (m_CurrentStack <= 0)
                return;

            m_RemainingDuration -= Time.deltaTime;

            m_OnDurationUpdated?.Invoke(
                Mathf.Clamp01(m_RemainingDuration / m_Config.Duration));

            if (m_RemainingDuration <= 0f)
            {
                RemoveOneStack();
            }
        }

        #endregion
    }
}