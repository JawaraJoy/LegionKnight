using Rush;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Badge", menuName = "Legion Knight/Badge")]
    public class BadgeConfig : CollectibleConfig
    {
        [SerializeField]
        private BadgeUpgrade[] m_Upgrade;
        [SerializeField] private ProfileBadgeGoal m_Goal;
        public ProfileBadgeGoal Goal => m_Goal;
        public BadgeUpgrade[] Upgrade => m_Upgrade;

        public void AddUnClaimedReward(int amount)
        {
            if (Player.Instance.BadgeManager.HasBadge(this, out var content))
            {
                content.AddUnClaimedReward(amount);
            }
        }
        public void SetUpgradeCurrentLevel(int level)
        {
            if (Player.Instance.BadgeManager.HasBadge(this, out var content))
            {
                content.SetCurrentUpgradeLevel(level);
            }
        }
        public void ClaimUpgradeReward()
        {
            if (Player.Instance.BadgeManager.HasBadge(this, out var content))
            {
                Player.Instance.BadgeManager.ClaimReward(this);
            }
        }
    }

    [System.Serializable]
    public class BadgeUpgrade
    {
        [SerializeField]
        private string m_Label;
        [SerializeField, TextArea]
        private string m_Description;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private LootField[] m_RewardOnUnlock;
        [SerializeField] private int m_Target;
        public int Target => m_Target;
        public string Label => m_Label;
        public string Description => m_Description;
        public Sprite Icon => m_Icon;
        public LootField[] RewardOnUnlock => m_RewardOnUnlock;
    }
}
