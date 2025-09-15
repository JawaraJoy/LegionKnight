using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class ExpTable
    {
        [SerializeField]
        private int m_CurrentMaxExp;
        [SerializeField]
        private LootDefinition m_RewardLevelReached;
        public int CurrentMaxExp => m_CurrentMaxExp;
        public LootDefinition RewardLevelReached => m_RewardLevelReached;
        public ExpTable(int currentMaxExp, LootDefinition rewardLevelReached)
        {
            m_CurrentMaxExp = currentMaxExp;
            m_RewardLevelReached = rewardLevelReached;
        }
    }
}
