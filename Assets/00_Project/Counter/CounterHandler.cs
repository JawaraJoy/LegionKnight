using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public enum CounterType
    {
        GreaterThan,
        LessThan,
        EqualTo,
        GreaterThanOrEqualTo,
        LessThanOrEqualTo,
    }
    [System.Serializable]
    public class Counter
    {
        [SerializeField]
        private CounterDefinition m_Definition;
        private int m_CurrentCount;
        private bool m_Triggered;
        [SerializeField]
        private UnityEvent m_OnCountMet;
        [SerializeField]
        private UnityEvent<int> m_OnCountChanged;
        [SerializeField]
        private UnityEvent<float> m_OnCountChangedRate;

        public Counter(CounterDefinition definition)
        {
            m_Definition = definition;
            m_CurrentCount = 0;
        }
        public CounterDefinition Definition
        {
            get { return m_Definition; }
        }
        public CounterType CounterType
        {
            get { return m_Definition.Type; }
        }
        public int CurrentCount
        {
            get { return m_CurrentCount; }
        }
        private void ResetCountInternal()
        {
            m_CurrentCount = 0;
            OnCountChangedInvoke(m_CurrentCount);
        }
        public void ResetCount()
        {
            ResetCountInternal();
        }
        public void AddCount(int count)
        {
            m_CurrentCount += count;
            OnCountChangedInvoke(m_CurrentCount);
        }
        public void SetCount(int count)
        {
            m_CurrentCount = count;
            OnCountChangedInvoke(m_CurrentCount);
        }
        private void OnCountChangedInvoke(int count)
        {
            m_OnCountChanged.Invoke(count);
            OnCountChangedRateInvoke((float)count / (float)m_Definition.CountThreshold);
            OnCountMetInvoke();
        }
        private void OnCountChangedRateInvoke(float rate)
        {
            m_OnCountChangedRate.Invoke(rate);
        }
        private void OnCountMetInvoke()
        {
            if (IsCountMet())
            {
                if (m_Definition.TriggerOnce && m_Triggered)
                {
                    return;
                }

                m_OnCountMet.Invoke();
                m_Triggered = true;
                if (m_Definition.ResetOnMet)
                {
                    ResetCountInternal();
                }
            }
        }

        private bool IsCountMet()
        {
            switch (CounterType)
            {
                case CounterType.GreaterThan:
                    return m_CurrentCount > m_Definition.CountThreshold;
                case CounterType.LessThan:
                    return m_CurrentCount < m_Definition.CountThreshold;
                case CounterType.EqualTo:
                    return m_CurrentCount == m_Definition.CountThreshold;
                case CounterType.GreaterThanOrEqualTo:
                    return m_CurrentCount >= m_Definition.CountThreshold;
                case CounterType.LessThanOrEqualTo:
                    return m_CurrentCount <= m_Definition.CountThreshold;
                default:
                    return false;
            }

        }
    }
    public class CounterHandler : MonoBehaviour
    {
        [SerializeField]
        private Counter[] m_Counters;
        private Counter GetCounter(CounterDefinition definition)
        {
            foreach (var counter in m_Counters)
            {
                if (counter.Definition == definition)
                {
                    return counter;
                }
            }
            return null;
        }
        public void ResetCount(CounterDefinition definition)
        {
            var counter = GetCounter(definition);
            if (counter != null)
            {
                counter.ResetCount();
            }
        }
        public void AddCount(CounterDefinition definition, int count)
        {
            var counter = GetCounter(definition);
            if (counter != null)
            {
                counter.AddCount(count);
            }
        }
        public void SetCount(CounterDefinition definition, int count)
        {
            var counter = GetCounter(definition);
            if (counter != null)
            {
                counter.SetCount(count);
            }
        }
    }
}
