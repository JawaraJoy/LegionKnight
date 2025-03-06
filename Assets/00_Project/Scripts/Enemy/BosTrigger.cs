using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class BosTrigger : MonoBehaviour
    {
        [SerializeField]
        private int m_TouchDownThreshold = 0;

        private int m_TouchDownCount;
        [SerializeField]
        private UnityEvent<int> m_OnTouchDownCountChange = new();
        public void AddTouchDownCount(int add)
        {
            m_TouchDownCount += add;
            OnTouchDownCountInvoke();
        }
        public void SetTouchDownCount(int set)
        {
            m_TouchDownCount = set;
            OnTouchDownCountInvoke();
        }

        private void OnTouchDownCountInvoke()
        {
            m_OnTouchDownCountChange?.Invoke(m_TouchDownCount);

            if (m_TouchDownCount >= m_TouchDownThreshold)
            {
                if (GameManager.Instance.BosTriggered) return;
                GameManager.Instance.StartBos();
            }
        }
    }
}
