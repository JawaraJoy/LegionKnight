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
            Manager.OnTaskReset.AddListener(OnTaskResetInternal);

            PopulateTabsInternal();
            m_TabGroup?.Show();
        }

        protected override void HideInternal()
        {
            if (m_CloseButton != null) m_CloseButton.onClick.RemoveListener(Hide);

            Manager.OnTaskProgressUpdated.RemoveListener(OnTaskProgressUpdatedInternal);
            Manager.OnTaskCompleted.RemoveListener(OnTaskCompletedInternal);
            Manager.OnTaskClaimed.RemoveListener(OnTaskClaimedInternal);
            Manager.OnTaskReset.RemoveListener(OnTaskResetInternal);

            m_TabGroup?.Hide();
            base.HideInternal();
        }

        // ── Populate ──────────────────────────────────────────────────────────

        private void PopulateTabsInternal()
        {
            var catalogs = Manager.Catalogs;
            if (catalogs == null || m_CatalogTabEntries == null) return;

            for (int i = 0; i < m_CatalogTabEntries.Length && i < catalogs.Length; i++)
                m_CatalogTabEntries[i].Populate(catalogs[i]);
        }

        // ── Callbacks ─────────────────────────────────────────────────────────

        private void OnTaskProgressUpdatedInternal(QuestTaskConfig task) =>
            RefreshTaskInAllTabsInternal(task);

        private void OnTaskCompletedInternal(QuestTaskConfig task) =>
            RefreshTaskInAllTabsInternal(task);

        private void OnTaskClaimedInternal(QuestTaskConfig task, CollectibleResultData result)
        {
            RefreshTaskInAllTabsInternal(task);
            var resultPanel = CanvasManager.Instance.GetPanel<CollectibleResultPanel>();
            resultPanel?.Show(result);
        }

        private void OnTaskResetInternal(QuestTaskConfig task) =>
            RefreshTaskInAllTabsInternal(task);

        // Only the visible tab will actually refresh — others are hidden
        private void RefreshTaskInAllTabsInternal(QuestTaskConfig task)
        {
            if (m_CatalogTabEntries == null) return;
            foreach (var entry in m_CatalogTabEntries)
                entry.RefreshTaskIfVisible(task);
        }
    }
}