using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Counter", menuName = "Legion Knight/Counter", order = 1)]
    public class CounterDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_CounterName;
        [SerializeField]
        private CounterType m_CounterType;
        [SerializeField]
        private bool m_ResetOnMet = true;
        [SerializeField]
        private bool m_TriggerOnce = false;
        [SerializeField]
        private int m_CountThreshold;

        public bool ResetOnMet
        {
            get { return m_ResetOnMet; }
        }
        public bool TriggerOnce
        {
            get { return m_TriggerOnce; }
        }
        public string CounterName
        {
            get { return m_CounterName; }
        }
        public CounterType Type
        {
            get { return m_CounterType; }
        }
        public int CountThreshold
        {
            get { return m_CountThreshold; }
        }

        public void AddCount(int count)
        {
            GameManager.Instance.AddCounter(this, count);
        }
        public void SetCount(int count)
        {
            GameManager.Instance.SetCounter(this, count);
        }
        public void ResetCount()
        {
            GameManager.Instance.ResetCounter(this);
        }
    }
}
