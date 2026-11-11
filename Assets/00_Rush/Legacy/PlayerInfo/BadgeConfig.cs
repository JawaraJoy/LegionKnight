using Rush;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Badge", menuName = "Legion Knight/Badge")]
    public class BadgeConfig : CollectibleConfig
    {
        [SerializeField]
        private BadgeUpgrade[] m_Upgrade;
        public BadgeUpgrade[] Upgrade => m_Upgrade;

        public void AddUnClaimedReward(int amount)
        {
            Debug.Log("ttttt");
            Debug.Log(amount);

            if (Player.Instance.BadgeManager.HasBadge(this, out var content))
            {
                content.AddUnClaimedReward(amount);
            }
        }
        public void SetUpgradeCurrentLevel(int level)
        {
            Debug.Log("sssss");
            Debug.Log(level);

            if (Player.Instance.BadgeManager.HasBadge(this, out var content))
            {
                content.SetCurrentUpgradeLevel(level);
            }
        }
        public void ClaimUpgradeReward()
        {
            if (Player.Instance.BadgeManager.HasBadge(this, out var content))
            {
                content.ClaimReward();
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
        public string Label => m_Label;
        public string Description => m_Description;
        public Sprite Icon => m_Icon;
        public LootField[] RewardOnUnlock => m_RewardOnUnlock;
    }
}
