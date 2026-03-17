using MoreMountains.Tools;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class Attacker : MonoBehaviour, IHasAttacker
    {
        [SerializeField]
        private AttackerField m_AttackerField;
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnAttackStart;
        public UnityEvent<AbilityContext> OnAttackStart => m_OnAttackStart;
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnAttackDone;
        [SerializeField]
        private UnityEvent<IDamageable> m_OnAttackDelivered;
        [SerializeField]
        private UnityEvent<ITargetable> m_OnAttackDeliveredTarget;
        public UnityEvent<AbilityContext> OnAttackDone => m_OnAttackDone;
        private AbilityContext m_AbilityContext;
        public IAbilityContext AbilityContext => m_AbilityContext;
        public AttackerField AttackerField => m_AttackerField;

        public bool Initialized => m_AbilityContext.Initialized;

        public UnityEvent<IDamageable> OnAttackDelivered => m_OnAttackDelivered;
        public UnityEvent<ITargetable> OnAttackDeliveredTarget => m_OnAttackDeliveredTarget;
        public void Init(IAbilityContext context)
        {
            m_AbilityContext = new AbilityContext(context.AbilityDeliver, context.SkillContext);
            
            AbilityConfig config = context.AbilityDeliver.AbilityConfig;
            if (config is DamageAbilityConfig damageConfig)
            {
                float damage = AbilityUltility.GetFinalPowerAmount(m_AbilityContext);
                float damageBaseTargetMaxHP = damageConfig.DamageBasedTargetMaxHP;
                int damageRounded = Mathf.RoundToInt(damage);
                m_AttackerField = new AttackerField(damageRounded, damageBaseTargetMaxHP, damageConfig.DamageType);
            }
            
        }
        [SerializeField, MMReadOnly]
        private int m_AttackCount;
        private void OnAttackStartInvoke()
        {
            m_OnAttackStart?.Invoke(m_AbilityContext);

            Unit unitTaker = m_AbilityContext.SkillContext.ModuleContext.Unit;
            if (unitTaker.HasBind(out SkillController hasSkill))
            {
                AbilityUltility.OnSkillEventActivates(hasSkill, ForceActiveState.OnDeclareAttack);
            }
        }
        private void OnAttackDoneInvoke()
        {
            m_OnAttackDone?.Invoke(m_AbilityContext);
        }
    }
}
