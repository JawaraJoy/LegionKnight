using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class Charger : MonoBehaviour
    {
        [SerializeField]
        private int m_ChargeAmount = 0;
        public int ChargeAmount => m_ChargeAmount;
        private AbilityContext m_AbilityContext;
        public AbilityContext AbilityContext => m_AbilityContext;
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnChargeStart;
        [SerializeField]
        private UnityEvent<int> m_OnChargeAmount;
        public UnityEvent<AbilityContext> OnChargeStart => m_OnChargeStart;
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnChargeDone;
        public UnityEvent<AbilityContext> OnChargeDone => m_OnChargeDone;

        private ChargeAbilityConfig m_ChargeConfig;
        public void Init(AbilityContext context)
        {
            m_AbilityContext = context;
            m_ChargeAmount = Mathf.RoundToInt(AbilityUltility.GetFinalPowerAmount(context));
            if (m_AbilityContext.AbilityDeliver.AbilityConfig is ChargeAbilityConfig ChargeConfig)
            {
                m_ChargeConfig = ChargeConfig;
            }
        }

        /// <summary>
        /// Perform direct heal to target after delay.
        /// </summary>
        public void Charge(ITargetable target, float delay)
        {
            m_OnChargeStart?.Invoke(m_AbilityContext);
            StopAllCoroutines();
            StartCoroutine(Charging(target, delay));
            
        }

        private IEnumerator Charging(ITargetable target, float delay)
        {
            if (m_ChargeConfig == null)
            {
                Debug.LogError("Healer: HealAbilityConfig not found in AbilityDeliver.Config");
                yield break;
            }
            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            for (int i = 0; i < m_ChargeConfig.ChargeTickCount; i++)
            {
                if (target == null || !target.IsAlive)
                    break;

                Unit unit = target.ModuleContext.Unit;
                if (unit == null) break;

                m_OnChargeAmount?.Invoke(m_ChargeAmount);
                if (unit.HasBind(out SkillController skill))
                {
                    Skill[] skills = skill.Skills.ToArray();
                    foreach (Skill skillActivator in skills)
                    {
                        if (skillActivator.SkillConfig.Category == m_ChargeConfig.SkillCategoryToCharge)
                        {
                            skillActivator.AddCharge(m_ChargeAmount);
                        }
                    }
                }

                if (i < m_ChargeConfig.ChargeTickCount - 1 && m_ChargeConfig.ChargeTickInterval > 0f)
                    yield return new WaitForSeconds(m_ChargeConfig.ChargeTickInterval);
            }

            m_OnChargeDone?.Invoke(m_AbilityContext);
        }
    }
}
