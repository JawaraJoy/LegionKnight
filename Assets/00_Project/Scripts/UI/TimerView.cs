using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public enum TimerType
    {
        Minute,
        Hourly,
        Daily,
    }
    public partial class TimerView : UIView
    {
        [SerializeField]
        private TimerDefinition m_TimerDefinition;
        [SerializeField]
        private TimerType m_TimerType = TimerType.Hourly;
        [SerializeField]
        private TextMeshProUGUI m_Description;
        [SerializeField]
        private Image m_Icon;
        [SerializeField]
        private TextMeshProUGUI m_TimerText;
        [SerializeField]
        private UnityEvent m_OnTimeTrigger;
        [SerializeField]
        private UnityEvent m_OnNoYetTrigger;
        private void OnTimeTriggerInvoke()
        {
            m_OnTimeTrigger?.Invoke();
            HideInternal();
        }
        private void OnNoYetTriggerInvoke()
        {
            m_OnNoYetTrigger?.Invoke();
            ShowInternal();
        }
        public void Init()
        {
            InitInternal();
        }
        private void InitInternal()
        {
            if (m_TimerDefinition != null)
            {
                m_TimerDefinition.Init();
                m_TimerDefinition.CheckTimer(OnTimeTriggerInvoke, OnNoYetTriggerInvoke);
            }
            else
            {
                Debug.LogError("TimerDefinition is not assigned in TimerView.");

            }
            m_Description.text = m_TimerDefinition.Description;
            m_Icon.sprite = m_TimerDefinition.Icon;
            switch (m_TimerType)
            {
                case TimerType.Minute:
                    m_TimerText.text = TimerDefinition.GetRemainingTimeAsStringMinute(m_TimerDefinition.TimerId);
                    break;
                case TimerType.Hourly:
                    m_TimerText.text = TimerDefinition.GetRemainingTimeAsStringHour(m_TimerDefinition.TimerId);
                    break;
                case TimerType.Daily:
                    m_TimerText.text = TimerDefinition.GetRemainingTimeAsStringDay(m_TimerDefinition.TimerId);
                    break;
                default:
                    Debug.LogError("Invalid timer type.");
                    break;
            }
        }
    }
}
