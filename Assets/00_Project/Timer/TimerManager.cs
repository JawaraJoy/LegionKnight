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
        public DateTime GetResetTime(string id)
        {
            return m_TimerManager.GetResetTime(id);
        }
        public void SetResetTime(TimerDefinition defi, DateTime resetTime)
        {
            m_TimerManager.SetResetTime(defi, resetTime);
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
