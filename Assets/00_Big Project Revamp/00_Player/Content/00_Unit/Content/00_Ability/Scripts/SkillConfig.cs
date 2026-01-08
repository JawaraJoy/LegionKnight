using LegionKnight;
using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Skill", menuName = "Rush/Combat/Skill", order = 1)]
    public partial class SkillConfig : Configuration
    {
        [SerializeField]
        private ActivationField m_Activation;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private SkillActivator m_ActivatorPrefab;
        [SerializeField]
        private ProgressField m_LevelSet;
        [SerializeField]
        private DamageAbilityConfig[] m_AbilitySets;
        public ActivationField Activation => m_Activation;
        public Sprite Icon => m_Icon;
        public SkillActivator ActivatorPrefab => m_ActivatorPrefab;
        public ProgressField LevelSet => m_LevelSet;
        public DamageAbilityConfig[] AbilitySets => m_AbilitySets;
        public DamageAbilityConfig GetAbilityConfig(string id)
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
