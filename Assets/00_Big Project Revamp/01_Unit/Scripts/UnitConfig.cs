using LegionKnight;
using UnityEngine;

namespace Rush
{
    public abstract partial class UnitConfig : CollectibleConfig
    {
        [SerializeField]
        private Unit m_UnitPrefab;
        [SerializeField]
        private Vector3 m_UnitScale = Vector3.one;
        [SerializeField]
        private FactionConfig m_Faction;
        [SerializeField]
        private ProgressField m_ProgressLevel;
        [SerializeField]
        private StatsField m_MainStats;
        public Unit UnitPrefab => m_UnitPrefab;
        public FactionConfig Faction => m_Faction;
        public ProgressField Progression => m_ProgressLevel;
        public StatsField MainStats => m_MainStats;
        public Vector3 UnitScale => m_UnitScale;
    }
}
