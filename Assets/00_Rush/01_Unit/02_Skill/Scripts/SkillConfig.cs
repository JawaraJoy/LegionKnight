using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Skill_", menuName = "Rush/Combat/Skill", order = 1)]
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
        [SerializeField, Tooltip("Optional mark applied by other skills on hit. Its stack rule and follow-up supply this passive's description.")]
        private AllStatusEffectConfig m_OnHitMarkForDescription;
        public AllStatusEffectConfig OnHitMarkForDescription => m_OnHitMarkForDescription;
        [SerializeField, TextArea] private string m_DetailSummary;
        public string DetailSummary => m_DetailSummary;
        public string Description => GameplayDescription.Skill(this, m_LevelSet == null ? 1 : m_LevelSet.Level);
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
        private float m_MaxInteruptResist; // untuk menentukan jumlah maxinteruptdamage taken, formula maxhealth * this resist
        [SerializeField]
        private bool m_CooldownOnCastFail;

        public float CastDuration => m_CastDuration;
        public float MaxInteruptResist => m_MaxInteruptResist;
        public bool CooldownOnCastFail => m_CooldownOnCastFail;
    }
}
