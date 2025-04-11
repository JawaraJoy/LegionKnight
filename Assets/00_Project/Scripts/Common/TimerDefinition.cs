using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public abstract partial class TimerDefinition : ScriptableObject
    {
        [SerializeField]
        protected string m_TimerId = "DailyTimer";
        [SerializeField, TextArea]
        private string m_Description;
        [SerializeField]
        private Sprite m_Icon;
        public string TimerId => m_TimerId;
        public string Description => m_Description;
        public Sprite Icon => m_Icon;
        public abstract void StartTimer();
        public abstract void CheckTimer(UnityAction onTrigger);

        // Changed TimerHandler to a non-static type to fix CS0718
        private static readonly List<TimerHandler> m_TimerHandlers = new();

        private static TimerHandler GetTimerHandler(string timerId)
        {
            foreach (var handler in m_TimerHandlers)
            {
                if (handler.TimerId == timerId)
                {
                    return handler;
                }
            }
            return null;
        }
        public static TimeSpan GetRemainingTime(string timerId)
        {
            var handler = GetTimerHandler(timerId);
            if (handler != null)
            {
                return handler.GetRemainingTime();
            }
            Debug.LogError($"TimerHandler with ID {timerId} not found");
            return TimeSpan.Zero; // Return TimeSpan.Zero instead of DateTime.MinValue
        }
        public static string GetRemainingTimeAsStringMinute(string timerId)
        {
            var handler = GetTimerHandler(timerId);
            if (handler != null)
            {
                return handler.GetRemainingTimeAsStringMinute();
            }
            Debug.LogError($"TimerHandler with ID {timerId} not found");
            return "00m:00s"; // Return a default string instead of DateTime.MinValue
        }
        public static string GetRemainingTimeAsStringHour(string timerId)
        {
            var handler = GetTimerHandler(timerId);
            if (handler != null)
            {
                return handler.GetRemainingTimeAsStringHour();
            }
            Debug.LogError($"TimerHandler with ID {timerId} not found");
            return "00h:00m"; // Return a default string instead of DateTime.MinValue
        }
        public static string GetRemainingTimeAsStringDay(string timerId)
        {
            var handler = GetTimerHandler(timerId);
            if (handler != null)
            {
                return handler.GetRemainingTimeAsStringDay();
            }
            Debug.LogError($"TimerHandler with ID {timerId} not found");
            return "00d:00h"; // Return a default string instead of DateTime.MinValue
        }
        protected static void AddTimerHandlerInternal(TimerHandler handler)
        {
            if (handler == null)
            {
                Debug.LogError("TimerHandler is null");
                return;
            }
            if (GetTimerHandler(handler.TimerId) != null)
            {
                Debug.LogWarning($"TimerHandler with ID {handler.TimerId} already exists");
                return;
            }
            m_TimerHandlers.Add(handler);
        }
        public static void RemoveTimerHandler(string timerId)
        {
            var handler = GetTimerHandler(timerId);
            if (handler != null)
            {
                m_TimerHandlers.Remove(handler);
            }
            else
            {
                Debug.LogError($"TimerHandler with ID {timerId} not found");
            }
        }

        protected static DateTime GetResetTime(string timerId)
        {
            return GetTimerHandler(timerId)?.GetResetTime() ?? DateTime.MinValue;
        }
        protected virtual void SetResetTimeInternal(string timerId, DateTime resetTime)
        {
            GetTimerHandler(timerId)?.SetResetTime(resetTime);
        }
        public void SetResetTime(string timerId, DateTime resetTime)
        {
            if (string.IsNullOrEmpty(timerId))
            {
                Debug.LogError("Timer ID is null or empty");
                return;
            }
            SetResetTimeInternal(timerId, resetTime);
        }
    }

    // Introduced a non-static wrapper class to represent instances of TimerHandle

    public partial class TimerHandler
    {
        private readonly string m_TimerId = "DailyTimer";
        private DateTime m_ResetTime;
        public string TimerId => m_TimerId;
        public void SetResetTime(DateTime resetTime)
        {
            m_ResetTime = resetTime;
        }
        public DateTime GetResetTime()
        {
            return m_ResetTime;
        }
        public TimerHandler(string timerId, DateTime time)
        {
            m_TimerId = timerId;
            m_ResetTime = time;
        }
        /// <summary>
        /// Gets the remaining time between now and the reset time.
        /// </summary>
        /// <returns>A TimeSpan representing the remaining time.</returns>
        public TimeSpan GetRemainingTime()
        {
            return GetRemainingTimeInternal();
        }
        private TimeSpan GetRemainingTimeInternal()
        {
            DateTime now = DateTime.Now;

            // If the reset time is in the past, calculate for the next day
            return m_ResetTime - now;
        }
        public string GetRemainingTimeAsStringMinute()
        {
            TimeSpan remainingTime = GetRemainingTimeInternal();
            // Format the TimeSpan as mm:ss
            return string.Format("{0:D2}m:{1:D2}s",
                remainingTime.Minutes,
                remainingTime.Seconds);
        }
        public string GetRemainingTimeAsStringHour()
        {
            TimeSpan remainingTime = GetRemainingTimeInternal();

            // Format the TimeSpan as HH:mm:ss
            return string.Format("{0:D2}h:{1:D2}m",
                remainingTime.Hours,
                remainingTime.Minutes);
        }
        public string GetRemainingTimeAsStringDay()
        {
            TimeSpan remainingTime = GetRemainingTimeInternal();

            return string.Format("{0:D2}d:{1:D2}h",
                remainingTime.Days,
                remainingTime.Hours);
        }
    }
}
