using System;
using UnityEngine;

namespace LegionKnight
{
    [Serializable]
    public partial class TimeCounter
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_ResetDateString;
        [SerializeField]
        private string m_TimeRemainingString;

        public string Id => m_Id;
        public string ResetDateString => GetRemainingTimeInternal().ToString("yyyy-MM-dd HH:mm:ss");

        public TimeCounter(string id, string reset)
        {
            m_Id = id;
            m_ResetDateString = reset;
        }
        public void Init()
        {
            if (UnityService.Instance.HasData(m_Id))
            {
                // Load the timer data from UnityService
                m_ResetDateString = UnityService.Instance.GetData<string>(m_Id);
            }
            else
            {
                m_ResetDateString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                UnityService.Instance.SaveData(m_Id, m_ResetDateString);
            }
        }

        public void SetResetTime(DateTime resetTime)
        {
            m_ResetDateString = resetTime.ToString("yyyy-MM-dd HH:mm:ss");
            UnityService.Instance.SaveData(m_Id, m_ResetDateString);
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

        private DateTime GetDateTimeNowInternal()
        {
            return DateTime.Now;
        }
        private DateTime GetResetTimeInternal()
        {
            if (string.IsNullOrEmpty(m_ResetDateString))
            {
                m_ResetDateString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return DateTime.Parse(m_ResetDateString);
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
