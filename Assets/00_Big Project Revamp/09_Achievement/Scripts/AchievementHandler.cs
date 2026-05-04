using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class AchievementHandler : MonoBehaviour
    {
        [SerializeField] private AchievementCatalogConfig[] m_Catalogs;
        [SerializeField] private AchievementTracker m_Tracker;
        [SerializeField] private CollectibleControl m_CollectibleControl;

        [SerializeField] private UnityEvent<AchievementTaskConfig> m_OnTaskProgressUpdated;
        [SerializeField] private UnityEvent<AchievementTaskConfig> m_OnTaskCompleted;
        [SerializeField] private UnityEvent<AchievementTaskConfig, CollectibleResultData> m_OnTaskClaimed;
        [SerializeField] private UnityEvent<string> m_OnClaimFailed;

        public AchievementCatalogConfig[] Catalogs => m_Catalogs;
        public UnityEvent<AchievementTaskConfig> OnTaskProgressUpdated => m_OnTaskProgressUpdated;
        public UnityEvent<AchievementTaskConfig> OnTaskCompleted => m_OnTaskCompleted;
        public UnityEvent<AchievementTaskConfig, CollectibleResultData> OnTaskClaimed => m_OnTaskClaimed;
        public UnityEvent<string> OnClaimFailed => m_OnClaimFailed;

        // ── Public ────────────────────────────────────────────────────────────

        public void AddTaskCount(AchievementTaskConfig task, int amount = 1)
        {
            if (task == null || amount <= 0) return;

            var state = GetTaskState(task);

            // Achievement is permanent — once complete and claimed, no more progress
            if (state.IsComplete || state.IsClaimed) return;

            int current = m_Tracker.GetCount(task);
            int newCount = Mathf.Min(current + amount, task.TargetCount);
            bool completed = newCount >= task.TargetCount;

            m_Tracker.SaveCount(task, newCount);
            m_OnTaskProgressUpdated?.Invoke(task);

            if (completed) m_OnTaskCompleted?.Invoke(task);
        }

        public void Claim(AchievementTaskConfig task)
        {
            if (task == null) return;

            var state = GetTaskState(task);

            if (!state.CanClaim)
            {
                m_OnClaimFailed?.Invoke(!state.IsComplete
                    ? "Achievement is not completed yet."
                    : "Reward already claimed.");
                return;
            }

            m_CollectibleControl?.AddCollectible(task.RewardCollectible, task.RewardAmount);
            m_Tracker.SaveClaimed(task, true);

            var result = new CollectibleResultData();
            result.AddEntry(task.RewardCollectible, task.RewardAmount);
            m_OnTaskClaimed?.Invoke(task, result);
        }

        public AchievementTaskState GetTaskState(AchievementTaskConfig task)
        {
            int count = m_Tracker.GetCount(task);
            bool complete = count >= task.TargetCount;
            bool claimed = m_Tracker.IsClaimed(task);
            return new AchievementTaskState(task, count, complete, claimed);
        }

        public List<AchievementTaskState> GetTaskStates(AchievementCatalogConfig catalog)
        {
            var list = new List<AchievementTaskState>();
            if (catalog?.Tasks == null) return list;
            foreach (var task in catalog.Tasks)
                if (task != null) list.Add(GetTaskState(task));
            return list;
        }

        // Find which catalog a task belongs to
        public AchievementCatalogConfig FindCatalog(AchievementTaskConfig task)
        {
            if (m_Catalogs == null) return null;
            foreach (var catalog in m_Catalogs)
            {
                if (catalog?.Tasks == null) continue;
                foreach (var t in catalog.Tasks)
                    if (t == task) return catalog;
            }
            return null;
        }
    }
}