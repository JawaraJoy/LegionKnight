using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public abstract partial class TimerDefinition : ScriptableObject
    {
        [SerializeField]
        protected string m_TimerId = "DailyTimer";
        [SerializeField, TextArea]
        private string m_Description;
        [SerializeField]
        private Sprite m_Icon;
        public string TimerId => m_TimerId;
        public string Description => m_Description;
        public Sprite Icon => m_Icon;
        public abstract void StartTimer(UnityAction callback = null);
        public virtual void CheckTimer(UnityAction onTrigger, UnityAction onNotYet)
        {
            // Check if the current time is past the reset time
            if (IsTimeToResetInternal())
            {
                onTrigger?.Invoke();
                //StartTimer();
                Debug.Log($"{m_TimerId} reset triggered!");
            }
            else
            {
                onNotYet?.Invoke();
                Debug.Log($"It's not time for the {m_TimerId} reset yet.");
            }
        }
        public bool IsTimeToReset()
        {
            return IsTimeToResetInternal();
        }
        protected virtual bool IsTimeToResetInternal()
        {
            DateTime now = DateTime.Now;
            DateTime resetTime = Player.Instance.GetResetTime(m_TimerId);
            // Check if the current time is past the reset time
            return now >= resetTime;
        }
    }
}
