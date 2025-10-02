using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class BadgeHandler : MonoBehaviour
    {
        [SerializeField]
        private BadgeContent[] m_Badges;
        [SerializeField]
        private UnityEvent<BadgeContent> m_OnBadgeUnlocked;
        [SerializeField]
        private UnityEvent<BadgeContent> m_OnBadgeCurrentUpgradeLevelChanged;
        private BadgeContent GetBadgeContent(BadgeDefinition badge)
        {
            foreach (var badgeContent in m_Badges)
            {
                if (badgeContent.Definition == badge)
                {
                    return badgeContent;
                }
            }
            return null;
        }
        public BadgeContent[] GetAllBadges()
        {
            return m_Badges;
        }
        private bool HasBadgeInternal(BadgeDefinition defi, out BadgeContent content)
        {
            content = GetBadgeContent(defi);
            return content != null;
        }
        public bool HasBadge(BadgeDefinition defi, out BadgeContent content)
        {
            return HasBadgeInternal(defi, out content);
        }
        public void Init()
        {
            foreach (var badgeContent in m_Badges)
            {
                badgeContent.Init();
            }
        }
        public void UnlockBadge(BadgeDefinition defi)
        {
            if (HasBadge(defi, out var content) && !content.IsUnlocked)
            {
                content.UnlockBadge();
                m_OnBadgeUnlocked?.Invoke(content);
            }
        }
        public void AddCurrentUpgradeLevel(BadgeDefinition defi, int amount)
        {
            if (HasBadge(defi, out var content))
            {
                content.AddCurrentUpgradeLevel(amount);
                m_OnBadgeCurrentUpgradeLevelChanged?.Invoke(content);
            }
        }
        public void SetCurrentUpgradeLevel(BadgeDefinition defi, int level)
        {
            if (HasBadge(defi, out var content))
            {
                content.SetCurrentUpgradeLevel(level);
                m_OnBadgeCurrentUpgradeLevelChanged?.Invoke(content);
            }
        }
        public void ClaimReward(BadgeDefinition defi)
        {
            if (HasBadge(defi, out var content))
            {
                content.ClaimReward();
            }
        }
    }
    [System.Serializable]
    public class BadgeContent
    {
        [SerializeField]
        private BadgeDefinition m_Definition;
        [SerializeField]
        private bool m_IsUnlocked = false;
        [SerializeField]
        private int m_CurrentUpgradeLevel;
        [SerializeField]
        private bool m_RewardClaimed = false;
        [SerializeField]
        private UnityEvent m_OnUnlocked;
        public BadgeDefinition Definition => m_Definition;
        public bool IsUnlocked => m_IsUnlocked;
        public int CurrentUpgradeLevel => m_CurrentUpgradeLevel;

        private const string KEY_IS_UNLOCKED = "isunlocked";
        private const string KEY_REWARD_CLAIMED = "rewardclaimed";
        private const string KEY_CURRENT_UPGRADE_LEVEL = "currentupgradelevel";
        public void UnlockBadge()
        {
            m_IsUnlocked = true;
            m_OnUnlocked?.Invoke();
            UnityService.Instance.SaveData(KEY_IS_UNLOCKED, m_IsUnlocked);
            m_RewardClaimed = false;
            SetRewardClaimed(false);
        }

        private void SetRewardClaimed(bool claimed)
        {
            m_RewardClaimed = claimed;
            UnityService.Instance.SaveData(KEY_REWARD_CLAIMED, m_RewardClaimed);
        }
        private void OnValidate()
        {
            if (m_CurrentUpgradeLevel > m_Definition.Upgrade.Length)
            {
                m_CurrentUpgradeLevel = m_Definition.Upgrade.Length;
            }
            UnityService.Instance.SaveData(KEY_CURRENT_UPGRADE_LEVEL, m_CurrentUpgradeLevel);
            SetRewardClaimed(false);
        }
        public void AddCurrentUpgradeLevel(int amount)
        {
            m_CurrentUpgradeLevel += amount;
            OnValidate();
        }
        public void SetCurrentUpgradeLevel(int level)
        {
            m_CurrentUpgradeLevel = level;
            OnValidate();
        }
        public BadgeContent(BadgeDefinition badge)
        {
            m_Definition = badge;
            m_IsUnlocked = false;
            m_CurrentUpgradeLevel = 0;
            m_RewardClaimed = false;
        }
        public void Init()
        {
            bool hasKeyUnlocked = UnityService.Instance.HasData(KEY_IS_UNLOCKED);
            bool hasKeyCurrentUpgradeLevel = UnityService.Instance.HasData(KEY_CURRENT_UPGRADE_LEVEL);
            bool hasKeyRewardClaimed = UnityService.Instance.HasData(KEY_REWARD_CLAIMED);
            if (hasKeyUnlocked)
            {
                m_IsUnlocked = UnityService.Instance.GetData<bool>(KEY_IS_UNLOCKED);
            }
            if (hasKeyCurrentUpgradeLevel)
            {
                m_CurrentUpgradeLevel = UnityService.Instance.GetData<int>(KEY_CURRENT_UPGRADE_LEVEL);
            }
            if (hasKeyRewardClaimed)
            {
                m_RewardClaimed = UnityService.Instance.GetData<bool>(KEY_REWARD_CLAIMED);
            }
        }

        public void ClaimReward()
        {
            if (m_IsUnlocked && !m_RewardClaimed)
            {
                LootField[] reward = m_Definition.Upgrade[m_CurrentUpgradeLevel].RewardOnUnlock;
                foreach (var loot in reward)
                {
                    loot.DirectTakeLoot();
                }
                SetRewardClaimed(true);
            }
        }
    }
}
