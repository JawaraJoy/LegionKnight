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
        public override void StartTimer(UnityAction callback = null)
        {
            DateTime resetTime = new (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, m_ResetClockHour, 0, 0);

            if (DateTime.Now >= resetTime)
            {
                resetTime = resetTime.AddDays(1);
            }

            string resetDateString = resetTime.ToString("yyyy-MM-dd HH:mm:ss");
            Player.Instance.SetResetTime(this, resetTime);
            callback?.Invoke();
        }
    }
}
