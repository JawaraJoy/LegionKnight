using System;
using UnityEngine;

namespace LegionKnight
{
    [Serializable]
    public partial class TimeCounter
    {
        [SerializeField]
        private TimerDefinition m_Definition;
        [SerializeField]
        private string m_ResetDateString;
        [SerializeField]
        private string m_TimeRemainingString;

        private DateTime m_ResetDateTime;

        public string Id => m_Definition.TimerId;
        public string ResetDateString => GetRemainingTimeInternal().ToString("yyyy-MM-dd HH:mm:ss");

        public TimeCounter(TimerDefinition defi, DateTime reset)
        {
            m_Definition = defi;
            m_ResetDateTime = reset;
            m_ResetDateString = reset.ToString("yyyy-MM-dd HH:mm:ss");
        }
        protected bool IsTimeToResetInternal()
        {
            DateTime now = GetDateTimeNowInternal();
            // Check if the current time is past the reset time
            return now >= GetResetTimeInternal();
        }
        public void Init()
        {
            if (UnityService.Instance.HasData(m_Definition.TimerId))
            {
                // Load the timer data from UnityService
                m_ResetDateTime = UnityService.Instance.GetData<DateTime>(m_Definition.TimerId);
                m_ResetDateString = m_ResetDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                Debug.Log($"Timer {m_Definition} initialized with reset time: {m_ResetDateString}");
            }
            else
            {
                m_ResetDateString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                UnityService.Instance.SaveData(m_Definition.TimerId, DateTime.Now);
                Debug.Log($"Timer {m_Definition} initialized with default reset time: {m_ResetDateString}");
            }
        }

        public void SetResetTime(DateTime resetTime)
        {
            m_ResetDateString = resetTime.ToString("yyyy-MM-dd HH:mm:ss");
            // Save the reset time to UnityService
            m_ResetDateTime = resetTime;
            UnityService.Instance.SaveData(m_Definition.TimerId, m_ResetDateTime);
        }
        public string GetRemainingTimeAsString(TimerType timerType)
        {
            switch (timerType)
            {
                case TimerType.Minute:
                    return GetRemainingTimeAsStringMinute();
                case TimerType.Hourly:
                    return GetRemainingTimeAsStringHour();
                case TimerType.Daily:
                    return GetRemainingTimeAsStringDay();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public DateTime GetResetTime()
        {
            return GetResetTimeInternal();
        }
        private DateTime GetDateTimeNowInternal()
        {
            return DateTime.Now;
        }
        private DateTime GetResetTimeInternal()
        {
            m_ResetDateTime = UnityService.Instance.GetData<DateTime>(m_Definition.TimerId);
            return m_ResetDateTime;
        }
        private TimeSpan GetRemainingTimeInternal()
        {
            TimeSpan timeRemaining = GetResetTimeInternal() - GetDateTimeNowInternal();
            m_TimeRemainingString = timeRemaining.ToString();
            return timeRemaining;
        }
        
        private string GetRemainingTimeAsStringMinute()
        {
            TimeSpan remainingTime = GetRemainingTimeInternal();
            // Format the TimeSpan as mm:ss
            return string.Format("{0:D2}m:{1:D2}s",
                remainingTime.Minutes,
                remainingTime.Seconds);
        }
        private string GetRemainingTimeAsStringHour()
        {
            TimeSpan remainingTime = GetRemainingTimeInternal();
            // Format the TimeSpan as hh:mm:ss
            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                remainingTime.Hours,
                remainingTime.Minutes,
                remainingTime.Seconds);
        }
        private string GetRemainingTimeAsStringDay()
        {
            TimeSpan remainingTime = GetRemainingTimeInternal();
            // Format the TimeSpan as dd:hh:mm:ss
            return string.Format("{0:D2}d:{1:D2}h:{2:D2}m",
                remainingTime.Days,
                remainingTime.Hours,
                remainingTime.Minutes);
        }
    }
}
