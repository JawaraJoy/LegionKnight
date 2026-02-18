using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class Attacker : MonoBehaviour, IHasAttacker, IHasAbilityContext
    {
        [SerializeField]
        private AttackerField m_AttackerField;
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnAttackStart;
        public UnityEvent<AbilityContext> OnAttackStart => m_OnAttackStart;
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnAttackDone;
        public UnityEvent<AbilityContext> OnAttackDone => m_OnAttackDone;
        private AbilityContext m_AbilityContext;
        public IAbilityContext AbilityContext => m_AbilityContext;
        public AttackerField AttackerField => m_AttackerField;

        public bool Initialized => m_AbilityContext.Initialized;

        public void Init(IAbilityContext context)
        {
            m_AbilityContext = new AbilityContext(context.AbilityDeliver, context.SkillContext);
            
            AbilityConfig config = context.AbilityDeliver.Config;
            if (config is DamageAbilityConfig damageConfig)
            {
                float damage = AbilityUltility.GetFinalPowerAmount(m_AbilityContext);
                float damageBaseTargetMaxHP = damageConfig.DamageBasedTargetMaxHP;
                bool isFatalDamage = damageConfig.IsFatalDamage;
                bool isTrueDamage = damageConfig.IsTrueDamage;
                int damageRounded = Mathf.RoundToInt(damage);
                m_AttackerField = new AttackerField(damageRounded, damageBaseTargetMaxHP, isTrueDamage, isFatalDamage);
            }
            
        }
        private void OnAttackStartInvoke()
        {
            m_OnAttackStart?.Invoke(m_AbilityContext);
        }
        private void OnAttackDoneInvoke()
        {
            m_OnAttackDone?.Invoke(m_AbilityContext);
        }
    }
}
