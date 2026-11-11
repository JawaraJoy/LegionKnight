using System;
using LegionKnight;
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

        public DailySignInState GetState() => StateInternal(DateTime.Now);

        private DailySignInState StateInternal(DateTime now)
        {
            CheckAndApplyResetInternal(now);
            int currentDay = CurrentDayInternal(now);
            bool complete = currentDay >= m_Config.TotalDays;
            DateTime? lastClaim = m_Tracker.GetLastClaimedTime(m_Config);
            bool canClaim = !complete && (!lastClaim.HasValue || lastClaim.Value < DailyBoundaryInternal(now))
                && !m_Tracker.IsDayClaimed(m_Config, currentDay);
            DateTime cycleStart = m_Tracker.GetCycleStartTime(m_Config) ?? now;
            DateTime cycleEnd = NextCycleResetInternal(cycleStart);
            DateTime nextDay = DailyBoundaryInternal(now).AddDays(1);
            bool waitsForCalendar = complete && (!m_Config.LoopOnComplete || !AllDaysClaimedInternal());
            DateTime nextReward = waitsForCalendar || cycleEnd < nextDay ? cycleEnd : nextDay;
            return new DailySignInState(currentDay, canClaim, complete, cycleEnd, m_Config.TotalDays,
                cycleStart, nextReward);
        }

        public bool IsDayClaimed(int dayIndex) => m_Tracker.IsDayClaimed(m_Config, dayIndex);

        public void Claim() => ClaimInternal(DateTime.Now, null, null);

        public void Claim(int dayIndex, DateTime cycleStart) => ClaimInternal(DateTime.Now, dayIndex, cycleStart);

        private void ClaimInternal(DateTime now, int? expectedDay, DateTime? expectedCycle)
        {
            var state = StateInternal(now);
            if ((expectedCycle.HasValue && expectedCycle.Value != state.CycleStartTime)
                || (expectedDay.HasValue && expectedDay.Value != state.CurrentDay))
            {
                OnClaimFailedInvoke("This sign-in day has changed.");
                return;
            }
            if (!state.CanClaimToday)
            {
                OnClaimFailedInvoke(state.CycleComplete ? "Sign-in cycle is complete." : "Already claimed today.");
                return;
            }
            if (!TryBuildRewardInternal(state.CurrentDay, out CollectibleResultData result)) return;
            UnityService.Instance.LocalSave.ExecuteTransaction(() =>
            {
                result.GrantRewards(m_Config.BaseInfo.Id);
                m_Tracker.SaveDayClaimed(m_Config, state.CurrentDay);
                m_Tracker.SaveCurrentDay(m_Config, state.CurrentDay + 1);
                m_Tracker.SaveLastClaimedTime(m_Config, now);
            });
            OnClaimSuccessInvoke(result);
        }

        public void ClaimMissedDay(int dayIndex) => ClaimMissedDayInternal(dayIndex, null, DateTime.Now);

        public void ClaimMissedDay(int dayIndex, DateTime cycleStart) =>
            ClaimMissedDayInternal(dayIndex, cycleStart, DateTime.Now);

        private void ClaimMissedDayInternal(int dayIndex, DateTime? expectedCycle, DateTime now)
        {
            var state = StateInternal(now);
            if (expectedCycle.HasValue && expectedCycle.Value != state.CycleStartTime)
            {
                OnClaimFailedInvoke("This sign-in cycle has ended.");
                return;
            }
            if (dayIndex < 0 || dayIndex >= m_Config.TotalDays || dayIndex >= state.CurrentDay)
            {
                OnClaimFailedInvoke("Invalid missed reward.");
                return;
            }
            if (m_Tracker.IsDayClaimed(m_Config, dayIndex))
            {
                OnClaimFailedInvoke("Reward already claimed.");
                return;
            }
            if (!TryBuildRewardInternal(dayIndex, out CollectibleResultData result)) return;
            UnityService.Instance.LocalSave.ExecuteTransaction(() =>
            {
                result.GrantRewards(m_Config.BaseInfo.Id);
                m_Tracker.SaveDayClaimed(m_Config, dayIndex);
                m_Tracker.SaveMissedDayClaimed(m_Config, dayIndex);
                if (m_Config.LoopOnComplete && state.CycleComplete && AllDaysClaimedInternal())
                    m_Tracker.SaveLastClaimedTime(m_Config, now);
            });
            OnClaimSuccessInvoke(result);
        }

        private bool TryBuildRewardInternal(int dayIndex, out CollectibleResultData result)
        {
            result = null;
            DailySignInRewardEntry entry = m_Config.Rewards[dayIndex];
            if (entry == null || entry.Collectible == null || entry.Amount <= 0)
            {
                OnClaimFailedInvoke("This reward is unavailable.");
                return false;
            }
            result = new CollectibleResultData();
            result.AddEntry(entry.Collectible, entry.Amount);
            return true;
        }

        private bool AllDaysClaimedInternal()
        {
            for (int i = 0; i < m_Config.TotalDays; i++)
                if (!m_Tracker.IsDayClaimed(m_Config, i)) return false;
            return m_Config.TotalDays > 0;
        }

        private int CurrentDayInternal(DateTime now)
        {
            DateTime start = m_Tracker.GetCycleStartTime(m_Config) ?? now;
            int elapsed = Math.Max(0, (DailyBoundaryInternal(now).Date - DailyBoundaryInternal(start).Date).Days);
            return Mathf.Clamp(Math.Max(m_Tracker.GetCurrentDay(m_Config), elapsed), 0, m_Config.TotalDays);
        }

        private void CheckAndApplyResetInternal(DateTime now)
        {
            DateTime? start = m_Tracker.GetCycleStartTime(m_Config);
            if (!start.HasValue)
            {
                m_Tracker.SaveCycleStartTime(m_Config, now);
                OnCycleResetInvoke();
                return;
            }
            // Calendar resets apply to incomplete cycles too, independently of looping.
            if (now >= NextCycleResetInternal(start.Value))
            {
                m_Tracker.ResetCycle(m_Config, CurrentCycleStartInternal(now));
                OnCycleResetInvoke();
                return;
            }
            DateTime? last = m_Tracker.GetLastClaimedTime(m_Config);
            if (m_Config.LoopOnComplete && m_Config.TotalDays > 0
                && last.HasValue && last.Value < DailyBoundaryInternal(now)
                && CurrentDayInternal(now) >= m_Config.TotalDays && AllDaysClaimedInternal())
            {
                m_Tracker.ResetCycle(m_Config, now);
                OnCycleResetInvoke();
            }
        }

        private DateTime DailyBoundaryInternal(DateTime now)
        {
            DateTime boundary = now.Date.AddHours(m_Config.ResetHour).AddMinutes(m_Config.ResetMinute);
            return now < boundary ? boundary.AddDays(-1) : boundary;
        }

        private DateTime NextCycleResetInternal(DateTime from)
        {
            if (m_Config.ResetCycle == SignInResetCycle.Monthly)
            {
                DateTime candidate = new DateTime(from.Year, from.Month, Mathf.Clamp(m_Config.MonthlyResetDay, 1, 28))
                    .AddHours(m_Config.ResetHour).AddMinutes(m_Config.ResetMinute);
                return candidate <= from ? candidate.AddMonths(1) : candidate;
            }
            DayOfWeek target = (DayOfWeek)((int)m_Config.WeeklyResetDay % 7);
            DateTime next = from.Date.AddHours(m_Config.ResetHour).AddMinutes(m_Config.ResetMinute);
            while (next.DayOfWeek != target || next <= from) next = next.AddDays(1);
            return next;
        }

        private DateTime CurrentCycleStartInternal(DateTime now)
        {
            if (m_Config.ResetCycle == SignInResetCycle.Monthly)
            {
                DateTime candidate = new DateTime(now.Year, now.Month, Mathf.Clamp(m_Config.MonthlyResetDay, 1, 28))
                    .AddHours(m_Config.ResetHour).AddMinutes(m_Config.ResetMinute);
                return candidate > now ? candidate.AddMonths(-1) : candidate;
            }
            DayOfWeek target = (DayOfWeek)((int)m_Config.WeeklyResetDay % 7);
            DateTime boundary = DailyBoundaryInternal(now);
            while (boundary.DayOfWeek != target) boundary = boundary.AddDays(-1);
            return boundary;
        }

        private void OnClaimSuccessInvoke(CollectibleResultData result) => m_OnClaimSuccess?.Invoke(result);
        private void OnClaimFailedInvoke(string message) => m_OnClaimFailed?.Invoke(message);
        private void OnCycleResetInvoke() => m_OnCycleReset?.Invoke();
    }
}
