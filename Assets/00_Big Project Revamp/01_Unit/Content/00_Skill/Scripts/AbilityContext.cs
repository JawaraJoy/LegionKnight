using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class AbilityContext : IAbilityContext
    {
        private readonly ISkillContext m_SkillContext;
        [SerializeField, MMReadOnly]
        private AbilityDeliver m_Deliver;

        public ISkillContext SkillContext => m_SkillContext;
        public AbilityDeliver AbilityDeliver => m_Deliver;
        public bool Initialized => m_SkillContext.Initialized && m_Deliver != null;
        public AbilityContext(AbilityDeliver deliver, ISkillContext skillContext)
        {
            m_Deliver = deliver;
            m_SkillContext = skillContext;
        }
    }
}
