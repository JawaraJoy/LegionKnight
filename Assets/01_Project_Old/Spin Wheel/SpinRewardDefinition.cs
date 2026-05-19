// SpinRewardDefinition.cs
using Rush;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "SpinReward_", menuName = "Legion Knight/SpinWheel/Spin Reward")]
    public class SpinRewardDefinition : Configuration
    {
        [SerializeField, Range(1, 100)]
        private int m_Weight = 10; // higher = more likely to land

        [SerializeField]
        private LootChestDefinition m_Rewards;

        [SerializeField]
        private Sprite m_Icon;

        public int Weight => m_Weight;
        public LootChestDefinition Rewards => m_Rewards;
        public Sprite Icon => m_Icon;
        public string Id => m_BaseInfo.Id;
        public string DisplayName => m_BaseInfo.Name;
    }
}