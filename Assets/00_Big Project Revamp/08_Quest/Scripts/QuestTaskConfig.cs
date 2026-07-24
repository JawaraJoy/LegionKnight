using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "QuestTask_", menuName = "Rush/Quest/Task")]
    public class QuestTaskConfig : Configuration
    {
        [Header("Task")]
        [SerializeField] private int m_TargetCount = 1;

        [Header("Reward")]
        [SerializeField] private CollectibleConfig m_RewardCollectible;
        [SerializeField] private int m_RewardAmount = 1;

        [SerializeField, MMReadOnly]
        private QuestCatalogConfig m_QuestCatalogConfig;

        public int TargetCount => m_TargetCount;
        public CollectibleConfig RewardCollectible => m_RewardCollectible;
        public QuestCatalogConfig QuestCatalogConfig => m_QuestCatalogConfig;
        public int RewardAmount => m_RewardAmount;

        public void SetCatalog(QuestCatalogConfig QuestCatalogConfig)
        {
            m_QuestCatalogConfig = QuestCatalogConfig;
        }

        public void AddTaskCount(int amount)
        {
            RushPlayer.Instance.QuestManager.AddTaskCount(this, amount);
        }
    }
}