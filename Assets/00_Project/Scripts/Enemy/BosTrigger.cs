using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class BosTrigger : MonoBehaviour
    {
        private bool m_Active = true;
        [SerializeField]
        private int m_TouchDownThreshold = 0;

        private int m_TouchDownCount;
        [SerializeField]
        private UnityEvent m_OnTriggered = new();
        [SerializeField]
        private UnityEvent<float> m_OnTouchDownCountRateChange = new();


        private float GetTouchDownRate()
        {
            return (float)m_TouchDownCount / (float)m_TouchDownThreshold;
        }
        public void AddTouchDownCount(int add)
        {
            m_TouchDownCount += add;
            OnTouchDownCountInvoke();
        }
        public void SetTouchDownCount(int set)
        {
            SetTouchDownCountInternal(set);
        }
        private void SetTouchDownCountInternal(int set)
        {
            m_TouchDownCount = set;
            OnTouchDownCountInvoke();
        }
        private void SetTriggerActiveInternal(bool set)
        {
            m_Active = set;
        }
        public void SetTriggerActive(bool set)
        {
            SetTriggerActiveInternal(set);
        }

        private void OnTouchDownCountInvoke()
        {
            
            if (!m_Active) return;
            if (m_TouchDownCount >= m_TouchDownThreshold)
            {
                //if (GameManager.Instance.BosTriggered) return;
                GameManager.Instance.StartBos();
                m_OnTriggered?.Invoke();
                SetTriggerActive(false);
                SetTouchDownCountInternal(0);
            }
            m_OnTouchDownCountRateChange?.Invoke(GetTouchDownRate());
        }
    }
    public partial class LevelManagerAgent
    {
        public void ResetBoss()
        {
            GameManager.Instance.ResetBoss();
        }
    }
}
