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
        private LootDefinition m_RewardLevelReached;
        [SerializeField]
        private UnityEvent m_OnLevelUpEnter;
        public int CurrentMaxExp => m_CurrentMaxExp;
        public LootDefinition RewardLevelReached => m_RewardLevelReached;
        public UnityEvent OnLevelUpEnter => m_OnLevelUpEnter;
        public ExpTable(int currentMaxExp, LootDefinition rewardLevelReached)
        {
            m_CurrentMaxExp = currentMaxExp;
            m_RewardLevelReached = rewardLevelReached;
        }
    }
}
