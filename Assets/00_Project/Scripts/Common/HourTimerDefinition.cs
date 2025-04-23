using System;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Daily Timer", menuName = "Legion Knight/Timers/Hourly Timer")]
    public partial class HourTimerDefinition : TimerDefinition
    {
        [SerializeField]
        private int m_ResetHour = 1; // Reset every hour
        public override void CheckTimer(UnityAction onTrigger, UnityAction onNotYet)
        {
            DateTime now = DateTime.Now;
            DateTime resetTime = GetResetTime(m_TimerId);

            // Check if the current time is past the reset time
            if (now >= resetTime)
            {
                onTrigger?.Invoke();
                SetResetTimeInternal(m_TimerId, DateTime.Now.AddHours(m_ResetHour));
                Debug.Log("Hour reset triggered!");
            }
            else
            {
                onNotYet?.Invoke();

                Debug.Log("It's not time for the Hour reset yet.");
            }
        }

        protected override void StartTimer()
        {
            AddTimerHandlerInternal(new TimerHandler(m_TimerId, DateTime.Now.AddHours(m_ResetHour)));
        }
    }
}
