using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class QuestHandler : MonoBehaviour
    {
        [SerializeField] private QuestCatalogConfig[] m_Catalogs;
        [SerializeField] private QuestTracker m_Tracker;
        [SerializeField] private CollectibleControl m_CollectibleControl;

        [SerializeField] private UnityEvent<QuestTaskConfig> m_OnTaskProgressUpdated;
        [SerializeField] private UnityEvent<QuestTaskConfig> m_OnTaskCompleted;
        [SerializeField] private UnityEvent<QuestTaskConfig, CollectibleResultData> m_OnTaskClaimed;
        [SerializeField] private UnityEvent<string> m_OnClaimFailed;
        [SerializeField] private UnityEvent<QuestCatalogConfig> m_OnCatalogReset;

        public QuestCatalogConfig[] Catalogs => m_Catalogs;
        public UnityEvent<QuestTaskConfig> OnTaskProgressUpdated => m_OnTaskProgressUpdated;
        public UnityEvent<QuestTaskConfig> OnTaskCompleted => m_OnTaskCompleted;
        public UnityEvent<QuestTaskConfig, CollectibleResultData> OnTaskClaimed => m_OnTaskClaimed;
        public UnityEvent<string> OnClaimFailed => m_OnClaimFailed;
        public UnityEvent<QuestCatalogConfig> OnCatalogReset => m_OnCatalogReset;

        // ── Public ────────────────────────────────────────────────────────────

        public void AddTaskCount(QuestTaskConfig task, int amount = 1)
        {
            if (task == null || amount <= 0) return;

            // Find which catalog this task belongs to and check reset
            var catalog = FindCatalogForTaskInternal(task);
            if (catalog != null) CheckAndApplyResetInternal(catalog);

            var state = GetTaskState(task);
            if (state.IsComplete || state.IsClaimed) return;

            int current = m_Tracker.GetCount(task);
            int newCount = Mathf.Min(current + amount, task.TargetCount);
            bool completed = newCount >= task.TargetCount;

            m_Tracker.SaveCount(task, newCount);
            m_OnTaskProgressUpdated?.Invoke(task);

            if (completed) 
            {
                m_OnTaskCompleted?.Invoke(task);
                AnalyticService.Instance.MissionCompleted(task.BaseInfo.Name, task.QuestCatalogConfig.BaseInfo.Name);
            }
        }

        public void Claim(QuestTaskConfig task)
        {
            if (task == null) return;

            var catalog = FindCatalogForTaskInternal(task);
            if (catalog != null) CheckAndApplyResetInternal(catalog);

            var state = GetTaskState(task);

            if (!state.CanClaim)
            {
                m_OnClaimFailed?.Invoke(!state.IsComplete
                    ? "Task is not completed yet."
                    : "Reward already claimed.");
                return;
            }

            //m_CollectibleControl?.AddCollectible(task.RewardCollectible, task.RewardAmount);
            m_Tracker.SaveClaimed(task, true);

            var result = new CollectibleResultData();
            result.AddEntry(task.RewardCollectible, task.RewardAmount);
            m_OnTaskClaimed?.Invoke(task, result);
        }

        public QuestTaskState GetTaskState(QuestTaskConfig task)
        {
            int count = m_Tracker.GetCount(task);
            bool complete = count >= task.TargetCount;
            bool claimed = m_Tracker.IsClaimed(task);
            return new QuestTaskState(task, count, complete, claimed);
        }

        public List<QuestTaskState> GetTaskStates(QuestCatalogConfig catalog)
        {
            CheckAndApplyResetInternal(catalog);
            var list = new List<QuestTaskState>();
            if (catalog?.Tasks == null) return list;
            foreach (var task in catalog.Tasks)
                if (task != null) list.Add(GetTaskState(task));
            return list;
        }

        public QuestCatalogState GetCatalogState(QuestCatalogConfig catalog)
        {
            CheckAndApplyResetInternal(catalog);
            DateTime nextReset = GetNextResetTimeInternal(catalog);
            return new QuestCatalogState(catalog, nextReset);
        }

        // ── Reset ─────────────────────────────────────────────────────────────

        private void CheckAndApplyResetInternal(QuestCatalogConfig catalog)
        {
            DateTime? lastReset = m_Tracker.GetLastResetTime(catalog);
            DateTime now = DateTime.Now;
            DateTime nextReset = lastReset.HasValue
                ? GetNextResetTimeInternal(catalog, lastReset.Value)
                : DateTime.MinValue;

            if (!lastReset.HasValue || now >= nextReset)
            {
                m_Tracker.ResetCatalog(catalog);
                m_OnCatalogReset?.Invoke(catalog);
            }
        }

        // ── Time Helpers ──────────────────────────────────────────────────────

        private DateTime GetNextResetTimeInternal(QuestCatalogConfig catalog)
        {
            DateTime? lastReset = m_Tracker.GetLastResetTime(catalog);
            return GetNextResetTimeInternal(catalog, lastReset ?? DateTime.Now);
        }

        private DateTime GetNextResetTimeInternal(QuestCatalogConfig catalog, DateTime from)
        {
            return catalog.ResetCycle switch
            {
                QuestResetCycle.Daily => GetNextDailyResetInternal(catalog, from),
                QuestResetCycle.Weekly => GetNextWeeklyResetInternal(catalog, from),
                _ => GetNextDailyResetInternal(catalog, from)
            };
        }

        private DateTime GetNextDailyResetInternal(QuestCatalogConfig catalog, DateTime from)
        {
            DateTime candidate = new DateTime(
                from.Year, from.Month, from.Day,
                catalog.ResetHour, catalog.ResetMinute, 0);

            if (candidate <= from)
                candidate = candidate.AddDays(1);

            return candidate;
        }

        private DateTime GetNextWeeklyResetInternal(QuestCatalogConfig catalog, DateTime from)
        {
            int targetDow = (int)catalog.WeeklyResetDay;
            DayOfWeek target = targetDow == 7
                ? DayOfWeek.Sunday : (DayOfWeek)targetDow;

            DateTime candidate = new DateTime(
                from.Year, from.Month, from.Day,
                catalog.ResetHour, catalog.ResetMinute, 0);

            while (candidate.DayOfWeek != target || candidate <= from)
                candidate = candidate.AddDays(1);

            return candidate;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private QuestCatalogConfig FindCatalogForTaskInternal(QuestTaskConfig task)
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