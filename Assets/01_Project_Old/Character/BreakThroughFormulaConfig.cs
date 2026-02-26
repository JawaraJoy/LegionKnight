using Rush;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New BreakThrough", menuName = "Legion Knight/Character/BreakThrough")]
    public class BreakThroughFormulaConfig : ScriptableObject
    {
        [Header("Currencies")]
        [SerializeField] private ItemConfig m_ShardConfig;
        [SerializeField] private ItemConfig m_CoinConfig;

        [Header("Steps (0-based stars)")]
        [Tooltip("Index = Star. Example: Length 4 => stars 0..3 (max star = 3).")]
        [SerializeField] private BreakThroughStep[] m_BreakThroughSteps;

        public ItemConfig ShardConfig => m_ShardConfig;
        public ItemConfig CoinConfig => m_CoinConfig;

        /// <summary>
        /// Returns max star index. Example: steps length 4 => max star = 3.
        /// Returns -1 if config is invalid (null/empty).
        /// </summary>
        public int GetMaxStar()
        {
            if (m_BreakThroughSteps == null || m_BreakThroughSteps.Length == 0)
                return -1;

            return m_BreakThroughSteps.Length - 1;
        }

        /// <summary>
        /// True if the given star equals or exceeds max star (cannot break further).
        /// Invalid config => treated as max (returns true) to prevent progression errors.
        /// </summary>
        private bool IsMaxStarInternal(int star)
        {
            int maxStar = GetMaxStar();
            if (maxStar < 0) return true;     // invalid config => block
            return star >= maxStar;
        }
        public bool IsMaxStar(int star)
        {
            return IsMaxStarInternal(star);
        }

        /// <summary>
        /// True if star is valid AND has a next star available.
        /// </summary>
        private bool CanBreakFurther(int star)
        {
            return IsValidStar(star) && !IsMaxStarInternal(star);
        }

        /// <summary>
        /// Checks if star is within configured range (0..maxStar).
        /// </summary>
        private bool IsValidStar(int star)
        {
            return m_BreakThroughSteps != null
                   && m_BreakThroughSteps.Length > 0
                   && star >= 0
                   && star < m_BreakThroughSteps.Length;
        }

        /// <summary>
        /// Safe lookup for step by star index.
        /// </summary>
        private bool TryGetStep(int star, out BreakThroughStep step)
        {
            step = null;

            if (m_BreakThroughSteps == null || m_BreakThroughSteps.Length == 0)
            {
                Debug.LogError($"{name}: BreakThroughSteps is null/empty.");
                return false;
            }

            if (star < 0 || star >= m_BreakThroughSteps.Length)
            {
                Debug.LogError($"{name}: Invalid star level: {star}. Must be between 0 and {m_BreakThroughSteps.Length - 1}.");
                return false;
            }

            step = m_BreakThroughSteps[star];
            if (step == null)
            {
                Debug.LogError($"{name}: BreakThroughSteps[{star}] is null.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Level requirement to be at this star (or to unlock breaking from this star - depends on your design).
        /// </summary>
        public int GetLevelNeeded(int star)
        {
            return TryGetStep(star, out var step) ? step.LevelNeeded : -1;
        }

        public StatField GetStatBonus(int star)
        {
            return TryGetStep(star, out var step) ? step.StatBonus : null;
        }

        /// <summary>
        /// Checks ONLY the level requirement for breaking from the current star.
        /// Will return false if config invalid, star invalid, or already at max.
        /// </summary>
        public bool CanBreakByLevel(int currentStar, int currentLevel)
        {
            if (!CanBreakFurther(currentStar)) return false;

            if (!TryGetStep(currentStar, out var currentStep))
                return false;

            return currentLevel >= currentStep.LevelNeeded;
        }

        /// <summary>
        /// Cost to break from currentStar -> nextStar.
        /// Returns -1 if cannot break further / invalid.
        /// </summary>
        public int GetShardCostToBreak(int currentStar)
        {
            if (!CanBreakFurther(currentStar)) return -1;

            int nextStar = currentStar + 1;
            return TryGetStep(nextStar, out var nextStep) ? nextStep.ShardAmountToBreak : -1;
        }

        /// <summary>
        /// Cost to break from currentStar -> nextStar.
        /// Returns -1 if cannot break further / invalid.
        /// </summary>
        public int GetCoinCostToBreak(int currentStar)
        {
            if (!CanBreakFurther(currentStar)) return -1;

            int nextStar = currentStar + 1;
            return TryGetStep(nextStar, out var nextStep) ? nextStep.CoinAmountToBreak : -1;
        }

        /// <summary>
        /// Convenience: fetch both costs in one call.
        /// </summary>
        public bool TryGetBreakCosts(int currentStar, out int shardCost, out int coinCost)
        {
            shardCost = -1;
            coinCost = -1;

            if (!CanBreakFurther(currentStar)) return false;

            shardCost = GetShardCostToBreak(currentStar);
            coinCost = GetCoinCostToBreak(currentStar);
            return shardCost >= 0 && coinCost >= 0;
        }
    }

    [System.Serializable]
    public class BreakThroughStep
    {
        [SerializeField] private int m_LevelNeeded;
        [SerializeField] private int m_ShardAmountToBreak;
        [SerializeField] private int m_CoinAmountToBreak;
        [SerializeField] private StatField m_StatBonus;

        public int LevelNeeded => m_LevelNeeded;
        public int ShardAmountToBreak => m_ShardAmountToBreak;
        public int CoinAmountToBreak => m_CoinAmountToBreak;
        public StatField StatBonus => m_StatBonus;
    }
}