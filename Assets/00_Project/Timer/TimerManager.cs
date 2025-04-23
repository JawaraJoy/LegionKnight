using System;
using UnityEngine;

namespace LegionKnight
{
    public partial class TimerManager : Timer
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private TimerManager m_TimerManager;

        public void InitTimer()
        {
            m_TimerManager.Init();
        }
        public void SetResetTime(string id, DateTime resetTime)
        {
            m_TimerManager.SetResetTime(id, resetTime);
        }
        public string GetRemainingTimeAsString(string id, TimerType timerType)
        {
            return m_TimerManager.GetRemainingTimeAsString(id, timerType);
        }
        public void RemoveTimeCounter(TimeCounter timeCounter)
        {
            m_TimerManager.RemoveTimeCounter(timeCounter);
        }
    }
}
