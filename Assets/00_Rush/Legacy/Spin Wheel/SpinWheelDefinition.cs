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

        [SerializeField, Min(1)] private int m_FullTurns = 3;
        [SerializeField, Min(0.1f)] private float m_SpinDuration = 3f;
        [SerializeField, Min(0f)] private float m_ClaimDelay = 0.35f;
        [SerializeField] private bool m_SkipAnimationByDefault;

        public SpinRewardDefinition[] Rewards => m_Rewards;
        public int FreeSpinAmountEachDay => m_FreeSpinAmountEachDay;
        public int FreeDrawWatchAmount => m_FreeDrawWatchAmount;
        public int FullTurns => m_FullTurns;
        public float SpinDuration => m_SpinDuration;
        public bool SkipAnimationByDefault => m_SkipAnimationByDefault;
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
            foreach (var reward in m_Rewards)
            {
                if (reward != null && reward.Weight > 0) totalWeight += reward.Weight;
            }
            if (totalWeight <= 0) return -1;

            int roll = Random.Range(0, totalWeight);
            int cumulative = 0;
            for (int i = 0; i < m_Rewards.Length; i++)
            {
                var reward = m_Rewards[i];
                if (reward == null || reward.Weight <= 0) continue;
                cumulative += reward.Weight;
                if (roll < cumulative)
                    return i;
            }

            return -1;
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