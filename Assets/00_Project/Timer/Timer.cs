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
        private bool HasTimeCounter(TimerDefinition defi)
        {
            foreach (var timeCounter in m_TimeCounters)
            {
                if (timeCounter.Id == defi.TimerId)
                {
                    return true;
                }
            }
            return false;
        }
        public void SetResetTime(TimerDefinition defi, DateTime resetTime)
        {
            if (!HasTimeCounter(defi))
            {
                m_TimeCounters.Add(new TimeCounter(defi, resetTime));
            }
            else
            {
                GetTimeCounter(defi.TimerId)?.SetResetTime(resetTime);
            }
        }
        public DateTime GetResetTime(string id)
        {
            TimeCounter timeCounter = GetTimeCounter(id);
            if (timeCounter != null)
            {
                return timeCounter.GetResetTime();
            }
            Debug.LogError($"TimeCounter with ID {id} not found");
            return DateTime.Now;
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
