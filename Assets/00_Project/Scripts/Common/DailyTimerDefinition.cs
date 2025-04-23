using System;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Daily Timer", menuName = "Legion Knight/Timers/Daily Timer")]
    public partial class DailyTimerDefinition : TimerDefinition
    {
        [SerializeField]
        private int m_ResetClockHour = 15;
        protected override void StartTimer()
        {
            DateTime resetTime = new (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, m_ResetClockHour, 0, 0);
            AddTimerHandlerInternal(new (m_TimerId, resetTime));
        }
        public override void CheckTimer(UnityAction onTrigger, UnityAction onNotYet)
        {
            DateTime now = DateTime.Now;
            DateTime resetTime = GetResetTime(m_TimerId);

            // Check if the current time is past the reset time
            if (now >= resetTime)
            {
                onTrigger?.Invoke();
                SetResetTimeInternal(m_TimerId, new DateTime(now.Year, now.Month, now.Day + 1, m_ResetClockHour, 0, 0));
                Debug.Log("Daily reset triggered!");
            }
            else
            {
                onNotYet?.Invoke();
                Debug.Log("It's not time for the daily reset yet.");
            }
        }
    }
}
