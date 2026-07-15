
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class AbilityContext : IAbilityContext
    {
        private readonly ISkillContext m_SkillContext;
        private readonly IAbilityDeliver m_AbilityDeliver;

        public ISkillContext SkillContext => m_SkillContext;
        public IAbilityDeliver AbilityDeliver => m_AbilityDeliver;
        public bool Initialized => m_SkillContext.Initialized && m_AbilityDeliver != null;
        public AbilityContext(IAbilityDeliver deliver, ISkillContext skillContext)
        {
            m_AbilityDeliver = deliver;
            m_SkillContext = skillContext;
        }
    }
}
