using UnityEngine;

namespace Rush
{
    public class PlatformAbilityDeliverField : IAbilityDeliver
    {
        [SerializeField]
        private AbilityConfig m_AbilityConfig;
        [SerializeField]
        private AbilityContext m_AbilityContext;
        public AbilityConfig AbilityConfig => m_AbilityConfig;

        public IAbilityContext AbilityContext => m_AbilityContext;

        public Transform DeliverTransform => m_AbilityContext.SkillContext.ModuleContext.Module.transform;

        public void Activate()
        {
            m_AbilityContext.SkillContext.Skill.ForceActivate(m_AbilityConfig);
        }

        public void Init(AbilityConfig config, ISkillContext skillContext)
        {
            m_AbilityConfig = config;
            m_AbilityContext = new AbilityContext(this, skillContext);
        }
        
    }
}
