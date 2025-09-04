using System;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Minute", menuName = "Legion Knight/Timers/Minute")]
    public partial class MinuteTimerDefinition : TimerDefinition
    {
        [SerializeField]
        private int m_ResetMinute;
        public override void StartTimer(UnityAction callback = null)
        {
            DateTime resetTime = DateTime.Now.AddMinutes(m_ResetMinute);
            Player.Instance.SetResetTime(this, resetTime);
            callback?.Invoke();
        }
    }
}
