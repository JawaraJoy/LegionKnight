using UnityEngine;

namespace LegionKnight
{
    public class CounterManager : CounterHandler
    {
        
    }

    public partial class GameManager
    {
        [SerializeField]
        private CounterManager m_CounterManager;

        public void AddCounter(CounterDefinition definition, int count)
        {
            m_CounterManager.AddCount(definition, count);
        }
        public void SetCounter(CounterDefinition definition, int count)
        {
            m_CounterManager.SetCount(definition, count);
        }
        public void ResetCounter(CounterDefinition definition)
        {
            m_CounterManager.ResetCount(definition);
        }
    }
}
