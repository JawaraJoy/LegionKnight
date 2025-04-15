using System;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Daily Timer", menuName = "Legion Knight/Timers/Minute Timer")]
    public partial class MinuteTimerDefinition : TimerDefinition
    {
        [SerializeField]
        private int m_ResetMinute;
        public override void CheckTimer(UnityAction onTrigger)
        {
            DateTime now = DateTime.Now;
            DateTime resetTime = GetResetTime(m_TimerId);

            // Check if the current time is past the reset time
            if (now >= resetTime)
            {
                onTrigger?.Invoke();
                SetResetTimeInternal(m_TimerId, DateTime.Now.AddMinutes(m_ResetMinute));
                Debug.Log("Minute reset triggered!");
            }
            else
            {
                Debug.Log("It's not time for the Minute reset yet.");
            }
        }

        public override void StartTimer()
        {
            DateTime resetTime = DateTime.Now.AddMinutes(m_ResetMinute);
            AddTimerHandlerInternal(new(m_TimerId, resetTime));
        }
    }
}
