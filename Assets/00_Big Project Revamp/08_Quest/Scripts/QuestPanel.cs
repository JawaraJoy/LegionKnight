using UnityEngine;
using UnityEngine.UI;
using LegionKnight;

namespace Rush
{
    public class QuestPanel : PanelView
    {
        [SerializeField] private TabGroup m_TabGroup;
        [SerializeField] private QuestCatalogTabEntry[] m_CatalogTabEntries;
        [SerializeField] private Button m_CloseButton;

        private QuestManager Manager => RushPlayer.Instance.QuestManager;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (m_CloseButton != null) m_CloseButton.onClick.AddListener(Hide);

            Manager.OnTaskProgressUpdated.AddListener(OnTaskProgressUpdatedInternal);
            Manager.OnTaskCompleted.AddListener(OnTaskCompletedInternal);
            Manager.OnTaskClaimed.AddListener(OnTaskClaimedInternal);
            Manager.OnCatalogReset.AddListener(OnCatalogResetInternal);

            PopulateTabsInternal();
            m_TabGroup?.Show();
        }

        protected override void HideInternal()
        {
            if (m_CloseButton != null) m_CloseButton.onClick.RemoveListener(Hide);

            Manager.OnTaskProgressUpdated.RemoveListener(OnTaskProgressUpdatedInternal);
            Manager.OnTaskCompleted.RemoveListener(OnTaskCompletedInternal);
            Manager.OnTaskClaimed.RemoveListener(OnTaskClaimedInternal);
            Manager.OnCatalogReset.RemoveListener(OnCatalogResetInternal);

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

        // ── Callbacks ─────────────────────────────────────────────────────────

        private void OnTaskProgressUpdatedInternal(QuestTaskConfig task) =>
            RefreshTaskInTabsInternal(task);

        private void OnTaskCompletedInternal(QuestTaskConfig task) =>
            RefreshTaskInTabsInternal(task);

        private void OnTaskClaimedInternal(QuestTaskConfig task, CollectibleResultData result)
        {
            RefreshTaskInTabsInternal(task);
            var resultPanel = CanvasManager.Instance.GetPanel<ShopResultPanel>();
            resultPanel?.Show(result);
        }

        // When a catalog resets, refresh all tasks in the matching tab only
        private void OnCatalogResetInternal(QuestCatalogConfig catalog)
        {
            if (m_CatalogTabEntries == null) return;
            foreach (var entry in m_CatalogTabEntries)
            {
                if (entry.BelongsToCatalog(catalog))
                    entry.RefreshAllIfVisible();
            }
        }

        private void RefreshTaskInTabsInternal(QuestTaskConfig task)
        {
            if (m_CatalogTabEntries == null) return;
            foreach (var entry in m_CatalogTabEntries)
                entry.RefreshTaskIfVisible(task);
        }
    }
}