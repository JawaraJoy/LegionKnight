using LegionKnight;
using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Skill Activator", menuName = "Rush/Combat/Activator", order = 1)]
    public partial class SkillActivatorConfig : Configuration
    {
        // Passive/Ultimate/BasicAttack, dll
        [SerializeField]
        private SkillCategory m_Category;
        [SerializeField]
        private ActivationTriggerField m_Activation;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private SkillActivator m_ActivatorPrefab;
        [SerializeField]
        private ProgressField m_LevelSet;
        [SerializeField]
        private DamageAbilityConfig[] m_AbilitySets;
        public SkillCategory Category => m_Category;
        public ActivationTriggerField Trigger => m_Activation;
        public Sprite Icon => m_Icon;
        public SkillActivator ActivatorPrefab => m_ActivatorPrefab;
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
