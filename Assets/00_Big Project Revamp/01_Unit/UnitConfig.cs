using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Rush/Unit/Unit", order = 0)]
    public partial class UnitConfig : Configuration
    {
        [SerializeField]
        private StatsField m_MainStats;
        public StatsField MainStats => m_MainStats;
    }

    [System.Serializable]
    public partial class UnitContext
    {
        [SerializeField, MMReadOnly]
        private UnitConfig m_UnitConfig;
        [SerializeField, MMReadOnly]
        private Unit m_UnitObject;
        public UnitConfig UnitConfig => m_UnitConfig;
        public Unit UnitObject => m_UnitObject;
        public UnitContext(UnitConfig unitConfig, Unit unitObject)
        {
            m_UnitConfig = unitConfig;
            m_UnitObject = unitObject;
        }
    }
}
