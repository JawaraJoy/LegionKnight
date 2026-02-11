using LegionKnight;
using UnityEngine;

namespace Rush
{
    public abstract partial class UnitConfig : Configuration, IHasIcon
    {
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private Unit m_UnitPrefab;
        [SerializeField]
        private RarityConfig m_Rarity;
        [SerializeField]
        private FactionConfig m_Faction;
        [SerializeField]
        private ProgressField m_StartingLevel;
        [SerializeField]
        private StatsField m_MainStats;
        public Unit UnitPrefab => m_UnitPrefab;
        public RarityConfig Rarity => m_Rarity;
        public FactionConfig Faction => m_Faction;
        public ProgressField StartingLevel => m_StartingLevel;
        public StatsField MainStats => m_MainStats;

        public Sprite Icon => m_Icon;
    }
}
