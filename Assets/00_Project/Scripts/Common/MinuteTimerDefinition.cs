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

        protected override void StartTimer()
        {
            DateTime resetTime = DateTime.Now.AddMinutes(m_ResetMinute);
            Player.Instance.SetResetTime(this, resetTime);
        }
    }
}
