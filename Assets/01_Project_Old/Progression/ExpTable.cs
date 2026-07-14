using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public class ExpTable
    {
        [SerializeField]
        private int m_CurrentMaxExp;
        [SerializeField]
        private UnityEvent m_OnLevelUpEnter;
        public int CurrentMaxExp => m_CurrentMaxExp;
        public UnityEvent OnLevelUpEnter => m_OnLevelUpEnter;
        public ExpTable(int currentMaxExp)
        {
            m_CurrentMaxExp = currentMaxExp;
        }

        public void TakeLoots()
        {

        }
    }
}
