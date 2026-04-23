using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LegionKnight;

namespace Rush
{
    public class QuestPanel : PanelView
    {
        [SerializeField] private QuestTaskItemPool m_TaskItemPool;
        [SerializeField] private Button m_CloseButton;

        private QuestManager Manager => RushPlayer.Instance.QuestManager;

        // Keep reference to active items for targeted refresh
        private readonly List<QuestTaskItemUI> m_ActiveItems = new();

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (m_CloseButton != null) m_CloseButton.onClick.AddListener(Hide);

            Manager.OnTaskProgressUpdated.AddListener(OnTaskProgressUpdatedInternal);
            Manager.OnTaskCompleted.AddListener(OnTaskCompletedInternal);
            Manager.OnTaskClaimed.AddListener(OnTaskClaimedInternal);
            Manager.OnTaskReset.AddListener(OnTaskResetInternal);

            PopulateTasksInternal();
        }

        protected override void HideInternal()
        {
            if (m_CloseButton != null) m_CloseButton.onClick.RemoveListener(Hide);

            Manager.OnTaskProgressUpdated.RemoveListener(OnTaskProgressUpdatedInternal);
            Manager.OnTaskCompleted.RemoveListener(OnTaskCompletedInternal);
            Manager.OnTaskClaimed.RemoveListener(OnTaskClaimedInternal);
            Manager.OnTaskReset.RemoveListener(OnTaskResetInternal);

            m_TaskItemPool?.ReturnAll();
            m_ActiveItems.Clear();
            base.HideInternal();
        }

        // ── Populate ──────────────────────────────────────────────────────────

        private void PopulateTasksInternal()
        {
            if (m_TaskItemPool == null) return;
            m_TaskItemPool.ReturnAll();
            m_ActiveItems.Clear();

            var states = Manager.GetAllTaskStates();
            foreach (var state in states)
            {
                var item = m_TaskItemPool.Rent();
                item.Setup(state);
                m_ActiveItems.Add(item);
            }
        }

        // Refresh only the item that matches the task — no full repopulate
        private void RefreshTaskItemInternal(QuestTaskConfig task)
        {
            if (m_ActiveItems == null || Manager.Catalog?.Tasks == null) return;

            for (int i = 0; i < Manager.Catalog.Tasks.Length; i++)
            {
                if (Manager.Catalog.Tasks[i] != task) continue;
                if (i >= m_ActiveItems.Count) break;

                var state = Manager.GetTaskState(task);
                m_ActiveItems[i].Refresh(state);
                break;
            }
        }

        // ── Callbacks ─────────────────────────────────────────────────────────

        private void OnTaskProgressUpdatedInternal(QuestTaskConfig task) =>
            RefreshTaskItemInternal(task);

        private void OnTaskCompletedInternal(QuestTaskConfig task) =>
            RefreshTaskItemInternal(task);

        private void OnTaskClaimedInternal(QuestTaskConfig task, CollectibleResultData result)
        {
            RefreshTaskItemInternal(task);
            var resultPanel = CanvasManager.Instance.GetPanel<CollectibleResultPanel>();
            resultPanel?.Show(result);
        }

        private void OnTaskResetInternal(QuestTaskConfig task) =>
            RefreshTaskItemInternal(task);
    }
}