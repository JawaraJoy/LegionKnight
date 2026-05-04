using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "AchievementCatalog_", menuName = "Rush/Achievement/Catalog")]
    public class AchievementCatalogConfig : Configuration
    {
        [Header("Tab Display")]
        [SerializeField] private string m_TabLabel;

        [Header("Tasks")]
        [SerializeField] private AchievementTaskConfig[] m_Tasks;

        public string TabLabel => m_TabLabel;
        public AchievementTaskConfig[] Tasks => m_Tasks;
    }
}