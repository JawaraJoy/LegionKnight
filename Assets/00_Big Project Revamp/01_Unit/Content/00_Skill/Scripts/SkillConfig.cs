using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Skill", menuName = "Rush/Combat/Skill", order = 1)]
    public partial class SkillConfig : CollectibleConfig
    {
        // Passive/Ultimate/BasicAttack, dll
        [SerializeField]
        private SkillCategoryConfig m_Category;
        [SerializeField]
        private SkillActivationState m_InitializeState = SkillActivationState.Idle;
        [SerializeField]
        private ActivationTriggerField m_Activation;
        [SerializeField]
        private CastingField m_Casting;
        
        [SerializeField]
        private Skill m_ActivatorPrefab;
        [SerializeField]
        private ProgressField m_LevelSet;
        [SerializeField]
        private AbilityConfig[] m_AbilitySets;
        public SkillCategoryConfig Category => m_Category;
        public ActivationTriggerField Activation => m_Activation;
        public Skill ActivatorPrefab => m_ActivatorPrefab;
        public ProgressField LevelSet => m_LevelSet;
        public CastingField Casting => m_Casting;
        public AbilityConfig[] AbilitySets => m_AbilitySets;
        public SkillActivationState InitializeState => m_InitializeState;
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

    [System.Serializable]
    public class CastingField
    {
        [SerializeField]
        private float m_CastDuration;
        [SerializeField]
        private int m_MaxInteruptCount;
        [SerializeField]
        private bool m_CooldownOnCastFail;

        public float CastDuration => m_CastDuration;
        public int MaxInterruptCount => m_MaxInteruptCount;
        public bool CooldownOnCastFail => m_CooldownOnCastFail;
    }
}
