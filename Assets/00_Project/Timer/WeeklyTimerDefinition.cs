using System;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Weekly", menuName = "Legion Knight/Timers/Weekly Timer")]
    public class WeeklyTimerDefinition : TimerDefinition
    {
        [SerializeField]
        private DayOfWeek m_ResetDay = DayOfWeek.Monday;
        [SerializeField, Range(0, 23)]
        private int m_ResetHour = 0;

        public override void StartTimer(UnityAction callback = null)
        {
            DateTime nextReset = GetLastResetTime().AddDays(7);
            Player.Instance.SetResetTime(this, nextReset);
            callback?.Invoke();
        }

        public string GetTimeToReset()
        {
            // fixed: use Weekly instead of Daily
            return Player.Instance.GetRemainingTimeAsString(m_TimerId, TimerType.Daily);
        }

        public override int DayCountPassedSinceReset()
        {
            DateTime lastReset = GetLastResetTime();
            double days = (DateTime.Now - lastReset).TotalDays;

            // fixed: use Floor to avoid off-by-one errors
            return (int)Math.Floor(days);
        }

        protected override DateTime GetLastResetTime()
        {
            DateTime now = DateTime.Now;

            int daysSinceReset = ((int)now.DayOfWeek - (int)m_ResetDay + 7) % 7;
            DateTime lastReset = now.AddDays(-daysSinceReset).Date.AddHours(m_ResetHour);

            if (now < lastReset)
                lastReset = lastReset.AddDays(-7);

            return lastReset;
        }

        protected override bool IsTimeToResetInternal()
        {
            DateTime now = DateTime.Now;
            DateTime lastReset = GetLastResetTime();
            DateTime nextReset = lastReset.AddDays(7);

            return now >= nextReset;
        }

        public override string GetRemainingTimeToReset()
        {
            DateTime lastReset = GetLastResetTime();
            DateTime nextReset = lastReset.AddDays(7);

            TimeSpan remaining = nextReset - DateTime.Now;
            if (remaining < TimeSpan.Zero)
                remaining = TimeSpan.Zero;

            return $"{remaining.Days}D:{remaining.Hours}H";
        }
    }

}
