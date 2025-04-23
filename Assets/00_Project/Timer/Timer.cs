using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class Timer : MonoBehaviour
    {
        [SerializeField]
        private List<TimeCounter> m_TimeCounters = new();

        private TimeCounter GetTimeCounter(string id)
        {
            foreach (var timeCounter in m_TimeCounters)
            {
                if (timeCounter.Id == id)
                {
                    return timeCounter;
                }
            }
            Debug.LogError($"TimeCounter with ID {id} not found");
            return null;
        }
        public void Init()
        {
            foreach (var timeCounter in m_TimeCounters)
            {
                timeCounter.Init();
            }
        }
        public void SetResetTime(string id, DateTime resetTime)
        {
            TimeCounter timeCounter = new (id, resetTime.ToString("yyyy-MM-dd HH:mm:ss"));
            if (m_TimeCounters.Contains(timeCounter))
            {
                m_TimeCounters.Add(timeCounter);
                return;
            }
            if (timeCounter != null)
            {
                m_TimeCounters.Add(timeCounter);
                timeCounter.Init();
            }
            else
            {
                Debug.LogError("TimeCounter is null.");
            }
        }
        public string GetRemainingTimeAsString(string id, TimerType timerType)
        {
            return GetTimeCounter(id)?.GetRemainingTimeAsString(timerType);
        }
        public void RemoveTimeCounter(TimeCounter timeCounter)
        {
            if (m_TimeCounters.Contains(timeCounter))
            {
                m_TimeCounters.Remove(timeCounter);
            }
            else
            {
                Debug.LogError("TimeCounter not found in the list.");
            }
        }
    }
}
