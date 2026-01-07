using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class AbilityContext
    {
        [SerializeField, MMReadOnly]
        private SkillContext m_SkillContext;
        [SerializeField, MMReadOnly]
        private AbilityConfig m_AbilityConfig;
        [SerializeField, MMReadOnly]
        private AbilityDeliver m_Deliver;

        public SkillContext SkillContext => m_SkillContext;
        public AbilityConfig AbilityConfig=> m_AbilityConfig;
        public AbilityDeliver AbilityDeliver => m_Deliver;

        public AbilityContext(SkillContext skillContext, AbilityConfig config)
        {
            m_SkillContext = skillContext;
            m_AbilityConfig = config;
        }
        public void SetDeliver(AbilityDeliver deliver)
        {
            m_Deliver = deliver;
        }
    }
}
