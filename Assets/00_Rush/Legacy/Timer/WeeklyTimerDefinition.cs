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
            DateTime now = DateTime.Now;

            // Calculate this week’s scheduled reset time
            int daysUntilReset = ((int)m_ResetDay - (int)now.DayOfWeek + 7) % 7;
            DateTime scheduledReset = now.Date.AddDays(daysUntilReset).AddHours(m_ResetHour);

            // If today’s reset already passed, schedule for next week
            if (scheduledReset <= now)
                scheduledReset = scheduledReset.AddDays(7);

            Player.Instance.SetResetTime(this, scheduledReset);

            Debug.Log($"[{m_TimerId}] Next reset scheduled at: {scheduledReset}");

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
            return (int)Math.Floor(days);
        }

        public override DateTime GetLastResetTime()
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
            DateTime storedReset = Player.Instance.GetResetTime(m_TimerId);

            bool isReset = now >= storedReset;
            Debug.Log($"[{m_TimerId}] Now={now}, StoredReset={storedReset}, IsReset={isReset}");
            return isReset;
        }

        public override string GetRemainingTimeToReset()
        {
            DateTime resetTime = Player.Instance.GetResetTime(m_TimerId);
            TimeSpan remaining = resetTime - DateTime.Now;
            if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;

            return $"{remaining.Days}D:{remaining.Hours}H:{remaining.Minutes}M";
        }
    }

}
