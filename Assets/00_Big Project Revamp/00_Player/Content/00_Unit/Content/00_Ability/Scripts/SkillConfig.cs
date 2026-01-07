using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Skill", menuName = "Rush/Combat/Skill", order = 1)]
    public partial class SkillConfig : Configuration
    {
        [SerializeField]
        private ProgressField m_LevelSet;
        [SerializeField]
        private AbilityConfig[] m_AbilitySets;
        public ProgressField LevelSet => m_LevelSet;
        public AbilityConfig[] AbilitySets => m_AbilitySets;
        public AbilityConfig GetAbilityConfig(string id)
        {
            foreach (var config in m_AbilitySets)
            {
                if (config.BaseInfo.Id == id)
                {
                    return config;
                }
            }
            return null;
        }
    }
}
