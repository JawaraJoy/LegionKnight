using MoreMountains.Tools;
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
    }
}
