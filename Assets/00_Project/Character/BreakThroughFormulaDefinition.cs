using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New BreakThrough", menuName = "Legion Knight/Character/BreakThrough")]
    public class BreakThroughFormulaDefinition : ScriptableObject
    {
        [SerializeField]
        private CurrencyDefinition m_ShardDefi;
        [SerializeField]
        private BreakThroughStep[] m_BreakThroughSteps;

        public CurrencyDefinition ShardDefinition => m_ShardDefi;

        private BreakThroughStep GetStepInternal(int star)
        {
            if (star < 0 || star >= m_BreakThroughSteps.Length)
            {
                Debug.LogError($"Invalid star level: {star}. Must be between 0 and {m_BreakThroughSteps.Length - 1}.");
                return null;
            }
            return m_BreakThroughSteps[star];
        }

        public bool CanBreak(int star, int level)
        {
            int levelNeeded = GetStepInternal(star).LevelNeeded;
            return level >= levelNeeded;
        }
        public int GetLevelNeeded(int star)
        {
            if (star < 0 || star >= m_BreakThroughSteps.Length)
            {
                Debug.LogError($"Invalid star level: {star}. Must be between 0 and {m_BreakThroughSteps.Length - 1}.");
                return -1;
            }
            return GetStepInternal(star).LevelNeeded;
        }

        public Stat GetStatBonus(int star)
        {
            if (star < 0 || star >= m_BreakThroughSteps.Length)
            {
                Debug.LogError($"Invalid star level: {star}. Must be between 0 and {m_BreakThroughSteps.Length - 1}.");
                return null;
            }
            return GetStepInternal(star).StatBonus;
        }

        public int GetMaxStar()
        {
            return m_BreakThroughSteps.Length - 1;
        }
        public int GetShardAmountToBreak(int star)
        {
            int nextStar = star + 1;
            if (nextStar < 0 || nextStar >= m_BreakThroughSteps.Length)
            {
                Debug.LogError($"Invalid next star level: {nextStar}. Must be between 0 and {m_BreakThroughSteps.Length - 1}.");
                return -1;
            }
            return GetStepInternal(nextStar).ShardAmountToBreak;
        }
    }

    [System.Serializable]
    public class BreakThroughStep
    {
        [SerializeField]
        private int m_LevelNeeded;
        [SerializeField]
        private int m_ShardAmountToBreak;
        [SerializeField]
        private Stat m_StatBonus;

        public int LevelNeeded => m_LevelNeeded;
        public Stat StatBonus => m_StatBonus;
        public int ShardAmountToBreak => m_ShardAmountToBreak;
    }
}
