using LegionKnight;

using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Hero", menuName = "Rush/Unit/Hero")]
    public partial class HeroUnitConfig : UnitConfig
    {
        [SerializeField, Min(0)]
        private int m_StartingStars;
        [SerializeField]
        private LevelFormulaConfig m_LevelFormulaConfig;
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("m_BreakThroughFormulaConfig")]
        private BreakThroughFormulaConfig m_EvolutionConfig;
        [SerializeField]
        private Currency m_ItemDuplicateConverter;
        [SerializeField]
        private DeckConfig m_HeroDeckConfig;
        [SerializeField]
        private bool m_UseAsDefault = false;
        [SerializeField]
        private bool m_OwnedAtFirst = false;

        public int StartingStars => m_StartingStars;
        public int MaxStars => HeroEvolution.MaxStage;
        public bool UseAsDefault => m_UseAsDefault;
        public bool OwnedAtFirst => m_OwnedAtFirst;
        public DeckConfig HeroDeckConfig => m_HeroDeckConfig;
        public LevelFormulaConfig LevelFormulaConfig => m_LevelFormulaConfig;
        public BreakThroughFormulaConfig EvolutionConfig => m_EvolutionConfig;
        public Currency ItemDuplicateConverter => m_ItemDuplicateConverter;
    }
}

