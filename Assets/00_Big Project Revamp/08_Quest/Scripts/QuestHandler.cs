using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class QuestHandler : MonoBehaviour
    {
        [SerializeField] private QuestCatalogConfig m_Catalog;
        [SerializeField] private QuestTracker m_Tracker;
        [SerializeField] private CollectibleControl m_CollectibleControl;

        [SerializeField] private UnityEvent<QuestTaskConfig> m_OnTaskProgressUpdated;
        [SerializeField] private UnityEvent<QuestTaskConfig> m_OnTaskCompleted;
        [SerializeField] private UnityEvent<QuestTaskConfig, CollectibleResultData> m_OnTaskClaimed;
        [SerializeField] private UnityEvent<string> m_OnClaimFailed;
        [SerializeField] private UnityEvent<QuestTaskConfig> m_OnTaskReset;

        public QuestCatalogConfig Catalog => m_Catalog;
        public UnityEvent<QuestTaskConfig> OnTaskProgressUpdated => m_OnTaskProgressUpdated;
        public UnityEvent<QuestTaskConfig> OnTaskCompleted => m_OnTaskCompleted;
        public UnityEvent<QuestTaskConfig, CollectibleResultData> OnTaskClaimed => m_OnTaskClaimed;
        public UnityEvent<string> OnClaimFailed => m_OnClaimFailed;
        public UnityEvent<QuestTaskConfig> OnTaskReset => m_OnTaskReset;

        // ── Public ────────────────────────────────────────────────────────────

        // Call this from anywhere in the game when relevant action happens
        // e.g. after killing a monster: questHandler.AddTaskCount(monsterKillTask, 1)
        public void AddTaskCount(QuestTaskConfig task, int amount = 1)
        {
            if (task == null || amount <= 0) return;

            CheckAndApplyResetInternal(task);

            var state = GetTaskState(task);

            // Already complete or claimed — ignore additional progress
            if (state.IsComplete || state.IsClaimed) return;

            int current = m_Tracker.GetCount(task);
            int newCount = Mathf.Min(current + amount, task.TargetCount);
            bool completed = newCount >= task.TargetCount;

            m_Tracker.SaveCount(task, newCount);
            m_OnTaskProgressUpdated?.Invoke(task);

            if (completed)
                m_OnTaskCompleted?.Invoke(task);
        }

        public void Claim(QuestTaskConfig task)
        {
            if (task == null) return;

            CheckAndApplyResetInternal(task);

            var state = GetTaskState(task);

            if (!state.CanClaim)
            {
                if (!state.IsComplete)
                    m_OnClaimFailed?.Invoke("Task is not completed yet.");
                else
                    m_OnClaimFailed?.Invoke("Reward already claimed.");
                return;
            }

            m_CollectibleControl?.AddCollectible(
                task.RewardCollectible, task.RewardAmount);

            m_Tracker.SaveClaimed(task, true);

            var result = new CollectibleResultData();
            result.AddEntry(task.RewardCollectible, task.RewardAmount);
            m_OnTaskClaimed?.Invoke(task, result);
        }

        public QuestTaskState GetTaskState(QuestTaskConfig task)
        {
            CheckAndApplyResetInternal(task);

            int count = m_Tracker.GetCount(task);
            bool complete = count >= task.TargetCount;
            bool claimed = m_Tracker.IsClaimed(task);
            DateTime nextReset = GetNextResetTimeInternal(task);

            return new QuestTaskState(task, count, complete, claimed, nextReset);
        }

        public List<QuestTaskState> GetAllTaskStates()
        {
            var list = new List<QuestTaskState>();
            if (m_Catalog?.Tasks == null) return list;
            foreach (var task in m_Catalog.Tasks)
                if (task != null) list.Add(GetTaskState(task));
            return list;
        }

        // ── Reset ─────────────────────────────────────────────────────────────

        private void CheckAndApplyResetInternal(QuestTaskConfig task)
        {
            DateTime? lastReset = m_Tracker.GetLastResetTime(task);
            DateTime now = DateTime.Now;
            DateTime nextReset = lastReset.HasValue
                ? GetNextResetTimeInternal(task, lastReset.Value)
                : DateTime.MinValue;

            // First time or past reset time
            if (!lastReset.HasValue || now >= nextReset)
            {
                m_Tracker.ResetTask(task);
                m_OnTaskReset?.Invoke(task);
            }
        }

        // ── Time Helpers ──────────────────────────────────────────────────────

        private DateTime GetNextResetTimeInternal(QuestTaskConfig task)
        {
            DateTime? lastReset = m_Tracker.GetLastResetTime(task);
            return GetNextResetTimeInternal(task, lastReset ?? DateTime.Now);
        }

        private DateTime GetNextResetTimeInternal(QuestTaskConfig task, DateTime from)
        {
            return task.ResetCycle switch
            {
                QuestResetCycle.Daily => GetNextDailyResetInternal(task, from),
                QuestResetCycle.Weekly => GetNextWeeklyResetInternal(task, from),
                _ => GetNextDailyResetInternal(task, from)
            };
        }

        private DateTime GetNextDailyResetInternal(QuestTaskConfig task, DateTime from)
        {
            // Next occurrence of reset time after 'from'
            DateTime candidate = new DateTime(
                from.Year, from.Month, from.Day,
                task.ResetHour, task.ResetMinute, 0);

            if (candidate <= from)
                candidate = candidate.AddDays(1);

            return candidate;
        }

        private DateTime GetNextWeeklyResetInternal(QuestTaskConfig task, DateTime from)
        {
            int targetDow = (int)task.WeeklyResetDay;
            DayOfWeek target = targetDow == 7
                ? DayOfWeek.Sunday : (DayOfWeek)targetDow;

            DateTime candidate = new DateTime(
                from.Year, from.Month, from.Day,
                task.ResetHour, task.ResetMinute, 0);

            // Advance until we hit the target day after 'from'
            while (candidate.DayOfWeek != target || candidate <= from)
                candidate = candidate.AddDays(1);

            return candidate;
        }
    }
}