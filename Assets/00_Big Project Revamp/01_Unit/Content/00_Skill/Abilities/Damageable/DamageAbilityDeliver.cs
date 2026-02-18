using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public partial class DamageAbilityDeliver : AbilityDeliver
    {
        public override void Init(AbilityConfig config, SkillContext context)
        {
            base.Init(config, context);
            ISkill skill = m_AbilityContext.SkillContext.Skill;
            SkillTriggerState triggerState = skill.SkillConfig.Activation.TriggerState;
            if (triggerState == SkillTriggerState.OnDeclareAttack)
            {
                m_OnActivate.RemoveAllListeners();
                
                m_OnActivate.AddListener((x) => skill.ForceActivateAll());
            }
        }
    }
}
