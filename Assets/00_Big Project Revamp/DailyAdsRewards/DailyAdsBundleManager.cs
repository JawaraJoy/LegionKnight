using UnityEngine;

namespace Rush
{
    public class DailyAdsBundleManager : DailyAdsRewardHandler
    {
        
    }

    public partial class RushPlayer
    {
        [SerializeField]
        private DailyAdsBundleManager m_DailyAdsBundleManager;
        public DailyAdsBundleManager DailyAdsBundleManager => m_DailyAdsBundleManager;
    }
}
