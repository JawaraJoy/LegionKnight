using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public partial class DamageAbilityDeliver : AbilityDeliver
    {
        public override void Init(AbilityConfig config, ISkillContext context)
        {
            base.Init(config, context);
            ISkill skill = m_AbilityContext.SkillContext.Skill;
            ForceActiveState triggerState = skill.SkillConfig.Activation.ForceActiveState;
            if (triggerState == ForceActiveState.OnDeclareAttack)
            {
                m_OnActivate.RemoveAllListeners();
                
                m_OnActivate.AddListener((x) => skill.ForceActivateAll());
            }
        }
        protected void LookAtTargetInternal(List<ITargetable> targets)
        {
            if (targets == null || targets.Count == 0)
                return;

            ITargetable target = targets[0];
            if (target?.TargetTransform == null)
                return;

            Vector2 dir = target.TargetTransform.position - m_DeliverTransform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            m_DeliverTransform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
