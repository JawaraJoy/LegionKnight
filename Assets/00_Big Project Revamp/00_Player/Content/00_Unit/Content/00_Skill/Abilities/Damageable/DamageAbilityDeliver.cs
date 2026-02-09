using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public partial class DamageAbilityDeliver : AbilityDeliver
    {
        public override void Init(AbilityConfig config, SkillContext context)
        {
            base.Init(config, context);
            SkillActivator skillActivator = m_AbilityContext.SkillContext.Activator;
            SkillTriggerState triggerState = skillActivator.SkillConfig.Activation.TriggerState;
            if (triggerState == SkillTriggerState.OnDeclareAttack)
            {
                m_OnActivate.RemoveAllListeners();
                m_OnActivate.AddListener((x) => skillActivator.ForceActivateAll());
            }
        }
    }
}
