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

    
}
