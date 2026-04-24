using UnityEngine;

namespace Rush
{
    public class QuestCatalogTabEntry : MonoBehaviour
    {
        [SerializeField] private TabEntry m_TabEntry;
        [SerializeField] private QuestCatalogView m_CatalogView;
        [SerializeField] private QuestCatalogConfig m_CatalogConfig;

        public QuestCatalogConfig CatalogConfig => m_CatalogConfig;

        public void Populate(QuestCatalogConfig catalog)
        {
            m_CatalogConfig = catalog;
            m_CatalogView?.Populate(catalog);
        }

        public void RefreshTaskIfVisible(QuestTaskConfig task)
        {
            if (m_CatalogView == null || !m_CatalogView.IsShow) return;
            m_CatalogView.RefreshTask(task);
        }

        // Called when the entire catalog resets — refresh all tasks in this view
        public void RefreshAllIfVisible()
        {
            if (m_CatalogView == null || !m_CatalogView.IsShow) return;
            m_CatalogView.RefreshAllTasks();
        }

        public bool BelongsToCatalog(QuestCatalogConfig catalog) =>
            m_CatalogConfig == catalog;
    }
}