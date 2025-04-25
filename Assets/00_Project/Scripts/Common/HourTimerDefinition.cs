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

        public override void StartTimer(UnityAction callback = null)
        {
            Player.Instance.SetResetTime(this, DateTime.Now.AddHours(m_ResetHour));
            callback?.Invoke();
        }
    }
}
