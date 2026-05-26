using System;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class DailySignInHandler : MonoBehaviour
    {
        [SerializeField] private DailySignInConfig m_Config;
        [SerializeField] private DailySignInTracker m_Tracker;
        [SerializeField] private CollectibleControl m_CollectibleControl;

        [SerializeField] private UnityEvent<CollectibleResultData> m_OnClaimSuccess;
        [SerializeField] private UnityEvent<string> m_OnClaimFailed;
        [SerializeField] private UnityEvent m_OnCycleReset;

        public DailySignInConfig Config => m_Config;
        public UnityEvent<CollectibleResultData> OnClaimSuccess => m_OnClaimSuccess;
        public UnityEvent<string> OnClaimFailed => m_OnClaimFailed;
        public UnityEvent OnCycleReset => m_OnCycleReset;

        // ── Public ────────────────────────────────────────────────────────────

        public DailySignInState GetState()
        {
            CheckAndApplyResetInternal();

            int currentDay = m_Tracker.GetCurrentDay(m_Config);
            bool cycleComplete = currentDay >= m_Config.TotalDays;

            bool canClaim = false;

            if (!cycleComplete)
            {
                DateTime now = DateTime.Now;
                DateTime? lastClaimed = m_Tracker.GetLastClaimedTime(m_Config);

                DateTime currentReset = GetCurrentResetBoundaryInternal(now);

                // hanya bisa claim kalau:
                // - belum pernah claim
                // - atau sudah masuk reset berikutnya
                canClaim = !lastClaimed.HasValue || lastClaimed.Value < currentReset;
            }
            DateTime nextReset = GetNextResetTimeInternal();

            // If loop is on and cycle is complete, it was already reset above
            return new DailySignInState(
                currentDay, canClaim, cycleComplete, nextReset, m_Config.TotalDays);
        }
        public void ClaimMissedDay(int dayIndex)
        {
            CheckAndApplyResetInternal();

            int currentDay = m_Tracker.GetCurrentDay(m_Config);

            // hanya boleh claim hari sebelumnya
            if (dayIndex >= currentDay)
            {
                m_OnClaimFailed?.Invoke("Invalid missed reward.");
                return;
            }

            // sudah pernah di-claim?
            if (m_Tracker.IsMissedDayClaimed(m_Config, dayIndex))
            {
                m_OnClaimFailed?.Invoke("Reward already claimed.");
                return;
            }

            var entry = m_Config.Rewards[dayIndex];

            m_CollectibleControl?.AddCollectible(entry.Collectible, entry.Amount);

            var result = new CollectibleResultData();
            result.AddEntry(entry.Collectible, entry.Amount);

            m_Tracker.SaveMissedDayClaimed(m_Config, dayIndex);

            m_OnClaimSuccess?.Invoke(result);
        }
        public void Claim()
        {
            CheckAndApplyResetInternal();

            int currentDay = m_Tracker.GetCurrentDay(m_Config);

            // Safety: prevent out of range
            if (currentDay >= m_Config.TotalDays)
            {
                if (!m_Config.LoopOnComplete)
                {
                    m_OnClaimFailed?.Invoke("Sign-in cycle is complete.");
                    return;
                }
            }

            DateTime now = DateTime.Now;
            DateTime? lastClaimed = m_Tracker.GetLastClaimedTime(m_Config);

            // 🔒 VALIDASI: tidak boleh claim lebih dari sekali dalam 1 "hari reset"
            if (lastClaimed.HasValue)
            {
                DateTime currentReset = GetCurrentResetBoundaryInternal(now);

                if (lastClaimed.Value >= currentReset)
                {
                    m_OnClaimFailed?.Invoke("Already claimed today.");
                    return;
                }
            }

            // Ambil reward
            var entry = m_Config.Rewards[currentDay];

            // Give reward
            m_CollectibleControl?.AddCollectible(entry.Collectible, entry.Amount);

            var result = new CollectibleResultData();
            result.AddEntry(entry.Collectible, entry.Amount);

            // Advance day
            m_Tracker.SaveCurrentDay(m_Config, currentDay + 1);
            m_Tracker.SaveLastClaimedTime(m_Config, now);

            m_OnClaimSuccess?.Invoke(result);
        }
        private DateTime GetCurrentResetBoundaryInternal(DateTime now)
        {
            DateTime todayReset = new DateTime(
                now.Year, now.Month, now.Day,
                m_Config.ResetHour, m_Config.ResetMinute, 0);

            // Kalau sekarang belum lewat jam reset,
            // berarti masih dihitung sebagai "hari sebelumnya"
            if (now < todayReset)
                return todayReset.AddDays(-1);

            return todayReset;
        }   

        // ── Reset ─────────────────────────────────────────────────────────────

        // Checks if a reset should happen and applies it
        private void CheckAndApplyResetInternal()
        {
            DateTime? cycleStart = m_Tracker.GetCycleStartTime(m_Config);

            // First time ever — initialize cycle start
            if (cycleStart == null)
            {
                m_Tracker.SaveCycleStartTime(m_Config, DateTime.Now);
                m_OnCycleReset?.Invoke();
                return;
            }

            DateTime now = DateTime.Now;
            DateTime nextReset = GetNextResetTimeInternal(cycleStart.Value);

            // Not yet time to reset
            if (now < nextReset) return;

            int currentDay = m_Tracker.GetCurrentDay(m_Config);

            // Cycle not complete and loop is off — do not reset mid-cycle
            if (currentDay < m_Config.TotalDays && !m_Config.LoopOnComplete) return;

            // Apply reset
            DateTime newCycleStart = GetCurrentCycleStartInternal(now);
            m_Tracker.ResetCycle(m_Config, newCycleStart);
            m_OnCycleReset?.Invoke();
        }

        // ── Time Helpers ──────────────────────────────────────────────────────

        private bool HasClaimedTodayInternal()
        {
            DateTime? lastClaimed = m_Tracker.GetLastClaimedTime(m_Config);
            if (lastClaimed == null) return false;

            // "Today" is relative to the configured reset time
            // If reset hour is 05:00, then "today" starts at 05:00
            DateTime resetToday = GetTodayResetBoundaryInternal();
            return lastClaimed.Value >= resetToday;
        }

        // The reset boundary for the current calendar day
        private DateTime GetTodayResetBoundaryInternal()
        {
            DateTime now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day,
                m_Config.ResetHour, m_Config.ResetMinute, 0);
        }

        // Next reset time from now
        private DateTime GetNextResetTimeInternal() =>
            GetNextResetTimeInternal(
                m_Tracker.GetCycleStartTime(m_Config) ?? DateTime.Now);

        private DateTime GetNextResetTimeInternal(DateTime cycleStart)
        {
            return m_Config.ResetCycle switch
            {
                SignInResetCycle.Weekly => GetNextWeeklyResetInternal(cycleStart),
                SignInResetCycle.Monthly => GetNextMonthlyResetInternal(cycleStart),
                _ => GetNextWeeklyResetInternal(cycleStart)
            };
        }

        private DateTime GetNextWeeklyResetInternal(DateTime cycleStart)
        {
            // Find the next occurrence of the configured day-of-week at reset time
            int targetDow = (int)m_Config.WeeklyResetDay; // 1=Mon, 7=Sun
            // Convert to DayOfWeek (0=Sun, 1=Mon ... 6=Sat)
            DayOfWeek targetDayOfWeek = targetDow == 7
                ? DayOfWeek.Sunday
                : (DayOfWeek)targetDow;

            DateTime candidate = new DateTime(
                cycleStart.Year, cycleStart.Month, cycleStart.Day,
                m_Config.ResetHour, m_Config.ResetMinute, 0);

            // Advance until we hit the target day-of-week
            while (candidate.DayOfWeek != targetDayOfWeek || candidate <= cycleStart)
                candidate = candidate.AddDays(1);

            return candidate;
        }

        private DateTime GetNextMonthlyResetInternal(DateTime cycleStart)
        {
            int targetDay = Mathf.Clamp(m_Config.MonthlyResetDay, 1, 28);

            // Start from next month if we've already passed this month's reset day
            DateTime candidate = new DateTime(
                cycleStart.Year, cycleStart.Month, targetDay,
                m_Config.ResetHour, m_Config.ResetMinute, 0);

            if (candidate <= cycleStart)
                candidate = candidate.AddMonths(1);

            return candidate;
        }

        // Get the start of the current cycle period (for reset reference)
        private DateTime GetCurrentCycleStartInternal(DateTime now)
        {
            return m_Config.ResetCycle switch
            {
                SignInResetCycle.Weekly => GetCurrentWeekStartInternal(now),
                SignInResetCycle.Monthly => GetCurrentMonthStartInternal(now),
                _ => now
            };
        }

        private DateTime GetCurrentWeekStartInternal(DateTime now)
        {
            int targetDow = (int)m_Config.WeeklyResetDay;
            DayOfWeek target = targetDow == 7 ? DayOfWeek.Sunday : (DayOfWeek)targetDow;
            DateTime day = now.Date;

            // Walk back to find the most recent occurrence of target day
            while (day.DayOfWeek != target)
                day = day.AddDays(-1);

            return new DateTime(day.Year, day.Month, day.Day,
                m_Config.ResetHour, m_Config.ResetMinute, 0);
        }

        private DateTime GetCurrentMonthStartInternal(DateTime now)
        {
            int targetDay = Mathf.Clamp(m_Config.MonthlyResetDay, 1, 28);
            DateTime candidate = new DateTime(now.Year, now.Month, targetDay,
                m_Config.ResetHour, m_Config.ResetMinute, 0);

            if (candidate > now)
                candidate = candidate.AddMonths(-1);

            return candidate;
        }
    }
}