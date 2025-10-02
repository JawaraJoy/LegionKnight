using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Badge", menuName = "Legion Knight/Badge")]
    public class BadgeDefinition : ScriptableObject, IDescriptable
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_Label;
        [SerializeField, TextArea]
        private string m_Description;
        [SerializeField]
        private BadgeUpgrade[] m_Upgrade;
        public string Id => m_Id;
        public string Label => m_Label;
        public string Description => m_Description;
        public BadgeUpgrade[] Upgrade => m_Upgrade;

        public void Unlock()
        {
            if (Player.Instance.BadgeManager.HasBadge(this, out var content) || !content.IsUnlocked)
            {
                Player.Instance.BadgeManager.UnlockBadge(this);
            }
            
        }
        public void AddUpgradeCurrentLevel(int amount)
        {
            if (Player.Instance.BadgeManager.HasBadge(this, out var content))
            {
                content.AddCurrentUpgradeLevel(amount);
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
