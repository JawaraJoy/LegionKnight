using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class BadgeHandler : MonoBehaviour
    {
        [SerializeField]
        private BadgeContent[] m_Badges;
        [SerializeField]
        private UnityEvent<BadgeContent> m_OnBadgeUnlocked;
        [SerializeField]
        private UnityEvent<BadgeContent> m_OnBadgeCurrentUpgradeLevelChanged;
        [SerializeField]
        private UnityEvent<LootField[]> m_OnRewardClaimed;

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
                m_OnRewardClaimed?.Invoke(RewardOnUnlockInternal(content));
            }
        }
        private static LootField[] RewardOnUnlockInternal(BadgeContent content)
        {
            int currentLevel = content.CurrentUpgradeLevel;
            return content.Definition.Upgrade[currentLevel].RewardOnUnlock;
        }
        public static LootField[] RewardOnUnlock(BadgeContent defi)
        {
            return RewardOnUnlockInternal(defi);
        }
    }
    [System.Serializable]
    public partial class BadgeContent
    {
        [SerializeField]
        private BadgeDefinition m_Definition;
        [SerializeField]
        private bool m_IsUnlocked = false;
        [SerializeField]
        private int m_CurrentUpgradeLevel;
        [SerializeField]
        private UnityEvent m_OnUnlocked;

        [SerializeField]
        private int m_UnlclaimedReward = 0;
        public BadgeDefinition Definition => m_Definition;
        public bool IsUnlocked => m_IsUnlocked;
        public int UnClaimedReward => m_UnlclaimedReward;
        public int CurrentUpgradeLevel => m_CurrentUpgradeLevel;

        private string KEY_IS_UNLOCKED => "isunlocked" + m_Definition.Id;
        private string KEY_UNCLAIMEDREWARD => "unclaimedreward" + m_Definition.Id;
        private string KEY_CURRENT_UPGRADE_LEVEL => "currentupgradelevel" + m_Definition.Id;

        private AchievementNotifPanel m_NotifPanel;
        private AchievementNotifPanel GetNotifPanel()
        {
            if (m_NotifPanel == null)
            {
                m_NotifPanel = CanvasManager.Instance.GetPanel<AchievementNotifPanel>();
            }
            return m_NotifPanel;
        }
        private void UnlockBadge()
        {
            if (m_IsUnlocked) return;
            m_IsUnlocked = true;
            m_OnUnlocked?.Invoke();
            UnityService.Instance.SaveData(KEY_IS_UNLOCKED, m_IsUnlocked);
            GetNotifPanel().ShowNotif(this);
        }
        private void AddUnClaimedRewardInternal(int unclaimedCount)
        {
            m_UnlclaimedReward += unclaimedCount;
            UnityService.Instance.SaveData(KEY_UNCLAIMEDREWARD, m_UnlclaimedReward);
            GetNotifPanel().ShowNotif(this);
        }
        private void RemoveUnClaimedRewardInternal(int unclaimedCount)
        {
            m_UnlclaimedReward -= unclaimedCount;
            UnityService.Instance.SaveData(KEY_UNCLAIMEDREWARD, m_UnlclaimedReward);
        }
        public void AddUnClaimedReward(int unclaimedCount)
        {
            AddUnClaimedRewardInternal(unclaimedCount);
        }
        private void OnValidate()
        {
            if (m_CurrentUpgradeLevel > m_Definition.Upgrade.Length - 1)
            {
                m_CurrentUpgradeLevel = m_Definition.Upgrade.Length - 1;
            }
            UnityService.Instance.SaveData(KEY_CURRENT_UPGRADE_LEVEL, m_CurrentUpgradeLevel);
        }
        private void AddCurrentUpgradeLevelInternal(int amount)
        {
            m_CurrentUpgradeLevel += amount;
            OnValidate();
        }
        public void SetCurrentUpgradeLevel(int level)
        {
            if (level > m_CurrentUpgradeLevel)
            {
                m_CurrentUpgradeLevel = level;
                OnValidate();
            }
        }
        public BadgeContent(BadgeDefinition badge)
        {
            m_Definition = badge;
            m_IsUnlocked = false;
            m_CurrentUpgradeLevel = 0;
            m_UnlclaimedReward = 0;
        }
        public void Init()
        {
            bool hasKeyUnlocked = UnityService.Instance.HasData(KEY_IS_UNLOCKED);
            bool hasKeyCurrentUpgradeLevel = UnityService.Instance.HasData(KEY_CURRENT_UPGRADE_LEVEL);
            bool hasKeyRewardClaimed = UnityService.Instance.HasData(KEY_UNCLAIMEDREWARD);
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
                m_UnlclaimedReward = UnityService.Instance.GetData<int>(KEY_UNCLAIMEDREWARD);
            }
        }

        public void ClaimReward()
        {
            if (m_UnlclaimedReward > 0)
            {
                LootField[] reward = BadgeHandler.RewardOnUnlock(this);
                foreach (var loot in reward)
                {
                    loot.DirectTakeLoot();
                }
                RemoveUnClaimedRewardInternal(1);
                bool hasMaxLevel = m_CurrentUpgradeLevel >= m_Definition.Upgrade.Length - 1;
                if (!hasMaxLevel)
                {
                    if (m_IsUnlocked)
                    {
                        AddCurrentUpgradeLevelInternal(1);
                    }
                }
            }
            UnlockBadge();
        }
    }
}
