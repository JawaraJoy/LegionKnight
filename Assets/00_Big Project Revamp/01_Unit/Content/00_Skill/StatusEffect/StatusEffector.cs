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
        private Unit m_UnitTarget;
        [SerializeField, MMReadOnly]
        private StatusEffectContext m_Context;
        public StatusEffectConfig Config => m_Config;
        public StatusEffectContext Context => m_Context;
        public Unit Target => m_UnitTarget;
        [SerializeField, MMReadOnly]
        private float m_RemainingDuration;
        public bool IsActive => gameObject.activeInHierarchy;

        public void ApplyEffect(StatusEffectConfig effectConfig, AbilityContext context, Unit target)
        {
            m_Config = effectConfig;
            m_UnitTarget = target;
            m_Context = new StatusEffectContext(context, this);
            m_Config.ApplyEffect(m_UnitTarget);
            m_RemainingDuration = m_Config.Duration;
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
            m_OnApplied?.Invoke(m_Context);
        }
        private void DoneEffect()
        {
            m_Config.DoneEffect(m_UnitTarget);
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
            m_OnDone?.Invoke(m_Context);
            gameObject.SetActive(false);
        }
        public void CancelEffect()
        {
            DoneEffect();
        }

        public void Tick()
        {
            m_RemainingDuration -= Time.deltaTime;
            m_OnDurationUpdated?.Invoke(m_RemainingDuration / m_Config.Duration);
            if (m_RemainingDuration <= 0f)
            {
                m_RemainingDuration = 0f;
                DoneEffect();
            }
        }
    }
}
