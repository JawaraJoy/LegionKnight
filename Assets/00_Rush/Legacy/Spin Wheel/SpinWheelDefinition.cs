// SpinWheelDefinition.cs
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "SpinWheel_", menuName = "Legion Knight/SpinWheel/Wheel Definition")]
    public class SpinWheelDefinition : ScriptableObject
    {
        [Header("Rewards")]
        [SerializeField]
        private SpinRewardDefinition[] m_Rewards;

        [Header("Free Spin")]
        [SerializeField]
        private int m_FreeSpinAmountEachDay = 1;
        [SerializeField]
        private int m_FreeDrawWatchAmount = 3;

        [Header("Spin Animation")]
        [SerializeField, Min(8)]
        private int m_MinSpinStep = 40;
        [SerializeField, Min(0)]
        private int m_MinAdditionalSpinStep = 8;
        [SerializeField, Min(1)]
        private int m_MaxAdditionalSpinStep = 24;
        [SerializeField, Min(0.01f)]
        private float m_StartStepDelay = 0.06f;
        [SerializeField, Min(0f)]
        private float m_MidDelayGrowth = 0.04f;
        [SerializeField, Min(0f)]
        private float m_EndDelayGrowth = 0.12f;
        [SerializeField, Min(0f)]
        private float m_ClaimDelay = 1f;

        public SpinRewardDefinition[] Rewards => m_Rewards;
        public int FreeSpinAmountEachDay => m_FreeSpinAmountEachDay;
        public int FreeDrawWatchAmount => m_FreeDrawWatchAmount;
        public int MinSpinStep => m_MinSpinStep;
        public int MinAdditionalSpinStep => m_MinAdditionalSpinStep;
        public int MaxAdditionalSpinStep => m_MaxAdditionalSpinStep;
        public float StartStepDelay => m_StartStepDelay;
        public float MidDelayGrowth => m_MidDelayGrowth;
        public float EndDelayGrowth => m_EndDelayGrowth;
        public float ClaimDelay => m_ClaimDelay;

        /// <summary>
        /// Picks a reward index using weighted random. Higher weight = more likely.
        /// Returns -1 if no rewards defined.
        /// </summary>
        public int PickWeightedRewardIndex()
        {
            if (m_Rewards == null || m_Rewards.Length == 0)
            {
                Debug.LogError("[SpinWheelDefinition] No rewards defined.");
                return -1;
            }

            int totalWeight = 0;
            foreach (var r in m_Rewards)
                totalWeight += r.Weight;

            int roll = Random.Range(0, totalWeight);
            int cumulative = 0;
            for (int i = 0; i < m_Rewards.Length; i++)
            {
                cumulative += m_Rewards[i].Weight;
                if (roll < cumulative)
                    return i;
            }

            return m_Rewards.Length - 1;
        }

        public bool TryGetReward(string id, out SpinRewardDefinition reward)
        {
            foreach (var r in m_Rewards)
            {
                if (r.Id == id)
                {
                    reward = r;
                    return true;
                }
            }
            reward = null;
            return false;
        }
    }
}