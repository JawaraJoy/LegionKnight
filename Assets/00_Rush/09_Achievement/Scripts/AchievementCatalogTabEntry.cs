using UnityEngine;

namespace Rush
{
    public class AchievementCatalogTabEntry : MonoBehaviour
    {
        [SerializeField] private TabEntry m_TabEntry;
        [SerializeField] private AchievementCatalogView m_CatalogView;
        [SerializeField] private AchievementCatalogConfig m_CatalogConfig;

        public AchievementCatalogConfig CatalogConfig => m_CatalogConfig;

        public void Populate(AchievementCatalogConfig catalog)
        {
            m_CatalogConfig = catalog;
            m_CatalogView?.Populate(catalog);
        }

        public void RefreshTaskIfVisible(AchievementTaskConfig task)
        {
            if (m_CatalogView == null || !m_CatalogView.IsShown) return;
            m_CatalogView.RefreshTask(task);
        }
    }
}