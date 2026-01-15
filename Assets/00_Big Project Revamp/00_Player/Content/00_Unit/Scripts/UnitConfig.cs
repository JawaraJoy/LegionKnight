using UnityEngine;

namespace Rush
{
    public abstract partial class UnitConfig : Configuration
    {
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private RarityConfig m_Rarity;
        [SerializeField]
        private FactionConfig m_Faction;
        [SerializeField]
        private ProgressField m_StartingLevel;
        [SerializeField]
        private StatsField m_MainStats;
        public Sprite Icon => m_Icon;
        public RarityConfig Rarity => m_Rarity;
        public FactionConfig Faction => m_Faction;
        public ProgressField StartingLevel => m_StartingLevel;
        public StatsField MainStats => m_MainStats;
    }

    
}
