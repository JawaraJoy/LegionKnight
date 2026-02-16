using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class Healer : MonoBehaviour, IHealer, IHasAbilityContext
    {
        [SerializeField]
        private int m_HealAmount = 0;
        public int HealAmount => m_HealAmount;
        private AbilityContext m_AbilityContext;
        public AbilityContext AbilityContext => m_AbilityContext;
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnHealStart;
        [SerializeField]
        private UnityEvent<int> m_OnHealAmount;
        public UnityEvent<AbilityContext> OnHealStart => m_OnHealStart;
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnHealDone;
        public UnityEvent<AbilityContext> OnHealDone => m_OnHealDone;

        public bool Initialized => m_AbilityContext.Initialized;

        private HealAbilityConfig m_HealConfig;
        public void Init(AbilityContext context)
        {
            m_AbilityContext = context;
            m_HealAmount = Mathf.RoundToInt(AbilityUltility.GetFinalEffectAmount(context));
            if (m_AbilityContext.AbilityDeliver.Config is HealAbilityConfig healConfig)
            {
                m_HealConfig = healConfig;
            }
        }

        /// <summary>
        /// Perform direct heal to target after delay.
        /// </summary>
        public void Heal(Targetable target, float delay)
        {
            m_OnHealStart?.Invoke(m_AbilityContext);
            StopAllCoroutines();
            StartCoroutine(Healing(target, delay));
            
        }

        private IEnumerator Healing(Targetable target, float delay)
        {
            if (m_HealConfig == null)
            {
                Debug.LogError("Healer: HealAbilityConfig not found in AbilityDeliver.Config");
                yield break;
            }
            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            for (int i = 0; i < m_HealConfig.HealTickCount; i++)
            {
                if (target == null || !target.IsAlive)
                    break;

                if (target.HasBind(out IDamageable damageable))
                {
                    damageable.Heal(this);
                    m_OnHealAmount?.Invoke(m_HealAmount);
                }

                if (i < m_HealConfig.HealTickCount - 1 && m_HealConfig.HealTickInterval > 0f)
                    yield return new WaitForSeconds(m_HealConfig.HealTickInterval);
            }

            m_OnHealDone?.Invoke(m_AbilityContext);
        }
    }
}
