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
        protected override void ShowInternal()
        {
            base.ShowInternal();
            AdjustTimerView();
        }
        private void OnTimeTriggerInvoke()
        {
            m_OnTimeTrigger?.Invoke();
            HideInternal();
            Debug.Log("Time trigger invoked!");
        }
        private void OnNoYetTriggerInvoke()
        {
            m_OnNoYetTrigger?.Invoke();
            ShowInternal();
            Debug.Log("No yet trigger invoked!");
        }
        public void StartTimer()
        {
            m_TimerDefinition.StartTimer(OnTimeTriggerInvoke);
            InitInternal();
        }
        public void Init()
        {
            InitInternal();
        }
        private void InitInternal()
        {
            if (m_TimerDefinition != null)
            {
                m_TimerDefinition.CheckTimer(OnTimeTriggerInvoke, OnNoYetTriggerInvoke);
            }
            else
            {
                Debug.LogError("TimerDefinition is not assigned in TimerView.");

            }
            AdjustTimerView();
        }

        public void AdjustTimerView()
        {
            m_Description.text = m_TimerDefinition.Description;
            m_Icon.sprite = m_TimerDefinition.Icon;
            m_TimerText.text = Player.Instance.GetRemainingTimeAsString(m_TimerDefinition.TimerId, TimerType.Minute);
        }
    }
}
