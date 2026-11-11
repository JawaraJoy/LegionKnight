using UnityEngine;
using UnityEngine.UI;
using LegionKnight;

namespace Rush
{
    public class AchievementPanel : PanelView
    {
        [SerializeField] private TabGroup m_TabGroup;
        [SerializeField] private AchievementCatalogTabEntry[] m_CatalogTabEntries;
        [SerializeField] private Button m_CloseButton;

        private AchievementManager Manager => RushPlayer.Instance.AchievementManager;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (m_CloseButton != null) m_CloseButton.onClick.AddListener(Hide);

            Manager.OnTaskProgressUpdated.AddListener(OnTaskProgressUpdatedInternal);
            Manager.OnTaskCompleted.AddListener(OnTaskCompletedInternal);
            Manager.OnTaskClaimed.AddListener(OnTaskClaimedInternal);

            PopulateTabsInternal();
            m_TabGroup?.Show();
        }

        protected override void HideInternal()
        {
            if (m_CloseButton != null) m_CloseButton.onClick.RemoveListener(Hide);

            Manager.OnTaskProgressUpdated.RemoveListener(OnTaskProgressUpdatedInternal);
            Manager.OnTaskCompleted.RemoveListener(OnTaskCompletedInternal);
            Manager.OnTaskClaimed.RemoveListener(OnTaskClaimedInternal);

            m_TabGroup?.Hide();
            base.HideInternal();
        }

        private void PopulateTabsInternal()
        {
            var catalogs = Manager.Catalogs;
            if (catalogs == null || m_CatalogTabEntries == null) return;

            for (int i = 0; i < m_CatalogTabEntries.Length && i < catalogs.Length; i++)
                m_CatalogTabEntries[i].Populate(catalogs[i]);
        }

        private void OnTaskProgressUpdatedInternal(AchievementTaskConfig task) =>
            RefreshTaskInTabsInternal(task);

        private void OnTaskCompletedInternal(AchievementTaskConfig task) =>
            RefreshTaskInTabsInternal(task);

        private void OnTaskClaimedInternal(AchievementTaskConfig task, CollectibleResultData result)
        {
            RefreshTaskInTabsInternal(task);
            var resultPanel = CanvasManager.Instance.GetPanel<CollectibleResultPanel>();
            resultPanel?.Show(result);
        }

        private void RefreshTaskInTabsInternal(AchievementTaskConfig task)
        {
            if (m_CatalogTabEntries == null) return;
            foreach (var entry in m_CatalogTabEntries)
                entry.RefreshTaskIfVisible(task);
        }
    }
}