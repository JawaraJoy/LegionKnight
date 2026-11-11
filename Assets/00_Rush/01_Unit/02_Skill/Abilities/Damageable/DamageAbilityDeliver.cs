using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public partial class DamageAbilityDeliver : AbilityDeliver
    {
        private ISkill m_ForceActivationSkill;
public override void Init(AbilityConfig config, ISkillContext context)
        {
            base.Init(config, context);

            m_OnActivate.RemoveListener(OnActivateForceSkillInternal);
            m_ForceActivationSkill = m_AbilityContext.SkillContext.Skill;

            ForceActiveState triggerState = m_ForceActivationSkill.SkillConfig.Activation.ForceActiveState;
            if (triggerState == ForceActiveState.OnDeclareAttack)
            {
                m_OnActivate.AddListener(OnActivateForceSkillInternal);
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
    

private void OnActivateForceSkillInternal(AbilityContext context)
        {
            m_ForceActivationSkill?.ForceActivateAll();
        }

        private void OnDestroy()
        {
            m_OnActivate.RemoveListener(OnActivateForceSkillInternal);
        }
}
}
