using UnityEngine;

namespace Rush
{
    public class AegisConversion : AbilityDeliver, IStatModifierDeliver
    {
        public StatModifierConfig ModifierConfig => m_AbilityConfig as StatModifierConfig;
        protected override void ActivateInternal()
        {
            Unit owner = m_AbilityContext.SkillContext.ModuleContext.Unit;
            if (owner.HasBind(out StatController controller))
                controller.AddModifier(m_AbilityContext, controller);
            base.ActivateInternal();
        }
    }
}
