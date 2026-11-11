using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "AdsBundle_", menuName = "Rush/Ads/Ads Bundle", order = 0)]
    public class DailyAdsBundleConfig : CollectibleConfig
    {
        [SerializeField]
        private CollectibleConfig m_Reward;
        [SerializeField]
        private int m_RewardAmount;

        public CollectibleConfig Reward => m_Reward;
        public int RewardAmount => m_RewardAmount;

        public void GrantReward()
        {
            CollectibleControl.AddCollectibleStatic(m_BaseInfo.Id, m_Reward, m_RewardAmount);
        }
    }
}
