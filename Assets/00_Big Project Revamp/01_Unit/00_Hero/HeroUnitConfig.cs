using LegionKnight;

using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Hero", menuName = "Rush/Unit/Hero")]
    public partial class HeroUnitConfig : UnitConfig
    {
        [SerializeField, Min(0)]
        private int m_StartingStars;
        [SerializeField, Min(0)]
        private int m_MaxStars;
        [SerializeField]
        private LevelFormulaConfig m_LevelFormulaConfig;
        [SerializeField]
        private BreakThroughFormulaConfig m_BreakThroughFormulaConfig;
        [SerializeField]
        private Currency m_ItemDuplicateConverter;
        [SerializeField]
        private DeckConfig m_HeroDeckConfig;
        [SerializeField]
        private bool m_UseAsDefault = false;
        [SerializeField]
        private bool m_OwnedAtFirst = false;

        public int StartingStars => m_StartingStars;
        public int MaxStars => m_MaxStars;
        public bool UseAsDefault => m_UseAsDefault;
        public bool OwnedAtFirst => m_OwnedAtFirst;
        public DeckConfig HeroDeckConfig => m_HeroDeckConfig;
        public LevelFormulaConfig LevelFormulaConfig => m_LevelFormulaConfig;
        public BreakThroughFormulaConfig BreakThroughFormulaConfig => m_BreakThroughFormulaConfig;
        public Currency ItemDuplicateConverter => m_ItemDuplicateConverter;
    }
}
