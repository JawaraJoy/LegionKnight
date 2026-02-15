using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class AbilityContext : IAbilityContext
    {
        [SerializeField, MMReadOnly]
        private SkillContext m_SkillContext;
        [SerializeField, MMReadOnly]
        private AbilityDeliver m_Deliver;

        public SkillContext SkillContext => m_SkillContext;
        public AbilityDeliver AbilityDeliver => m_Deliver;
        public bool Initialized => m_SkillContext.Initialized && m_Deliver != null;
        public AbilityContext(AbilityDeliver deliver, SkillContext skillContext)
        {
            m_Deliver = deliver;
            m_SkillContext = skillContext;
        }
    }
}
