// SpinRewardDefinition.cs
using Rush;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "SpinReward_", menuName = "Legion Knight/SpinWheel/Spin Reward")]
    public class SpinRewardDefinition : Configuration
    {
        [SerializeField, Range(1, 100)]
        private int m_Weight = 10;

        [SerializeField]
        private CollectibleConfig m_Collectible;

        [SerializeField]
        private int m_Amount = 1;

        public int Weight => m_Weight;
        public CollectibleConfig Collectible => m_Collectible;
        public int Amount => m_Amount;
        public string Id => m_BaseInfo.Id;
        public string DisplayName => m_BaseInfo.Name;

        /// <summary>
        /// Buat CollectibleResultData dari reward ini,
        /// siap di-pass ke CollectibleResultPanel.Show().
        /// </summary>
        public CollectibleResultData BuildResultData()
        {
            var data = new CollectibleResultData();
            data.AddEntry(m_Collectible, m_Amount);
            return data;
        }
    }
}