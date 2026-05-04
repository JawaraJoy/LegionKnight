using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Achievement_", menuName = "Rush/Achievement/Task")]
    public class AchievementTaskConfig : Configuration
    {
        [Header("Task")]
        [SerializeField] private int m_TargetCount = 1;

        [Header("Reward")]
        [SerializeField] private CollectibleConfig m_RewardCollectible;
        [SerializeField] private int m_RewardAmount = 1;

        public int TargetCount => m_TargetCount;
        public CollectibleConfig RewardCollectible => m_RewardCollectible;
        public int RewardAmount => m_RewardAmount;
    }
}