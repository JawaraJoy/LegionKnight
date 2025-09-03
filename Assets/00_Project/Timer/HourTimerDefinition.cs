using System;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Hour", menuName = "Legion Knight/Timers/Hourly")]
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
