using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "QuestCatalog_", menuName = "Rush/Quest/Catalog")]
    public class QuestCatalogConfig : Configuration
    {
        [SerializeField] private QuestTaskConfig[] m_Tasks;
        public QuestTaskConfig[] Tasks => m_Tasks;
    }
}