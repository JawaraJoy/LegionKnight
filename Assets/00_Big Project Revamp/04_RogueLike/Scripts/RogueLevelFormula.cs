using LegionKnight;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "RogueLevelFormula", menuName = "Rush/RogueLike/RogueLevelFormula", order = 1)]
    public class RogueLevelFormula : ScriptableObject
    {
        [SerializeField]
        private float m_ExponentialGrowth = 2; // Example: 2 means the experience required for each level increases exponentially
        [SerializeField]
        private int m_FirstLevelExp = 100; // Experience required for the first level
        [SerializeField]
        private int m_MaxLevel = 100;
        [SerializeField]
        private List<int> m_ExpTable = new();
        public float ExponentialGrowth => m_ExponentialGrowth;

        public int FirstLevelExp => m_FirstLevelExp;
        public int MaxLevel => m_MaxLevel;

        [method: ContextMenu("Generate Exp Table")]
        public void GenerateExpTable()
        {
            m_ExpTable.Clear();
            int exp = m_FirstLevelExp;
            for (int i = 0; i < m_MaxLevel; i++)
            {
                m_ExpTable.Add(exp);
                exp = Mathf.FloorToInt(exp * m_ExponentialGrowth);
            }
        }

        private float GetLevelProgressionRateInternal(int currentLevel)
        {
            if (currentLevel > 0 && currentLevel <= m_ExpTable.Count)
            {
                return (float)currentLevel / m_ExpTable[currentLevel - 1];
            }
            return 0f;
        }
        public float GetLevelProgressionRate(int currentLevel)
        {
            return GetLevelProgressionRateInternal(currentLevel);
        }
        public int GetCurrentMaxExperience(int level)
        {
            if (level - 1 < m_ExpTable.Count)
                return m_ExpTable[level - 1];
            return m_ExpTable.Count > 0 ? m_ExpTable[^1] : m_FirstLevelExp;
        }
    }
}
