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
                if (badgeContent.Badge == badge)
                {
                    return badgeContent;
                }
            }
            return null;
        }
        private bool HasBadgeInternal(BadgeDefinition badge, out BadgeContent content)
        {
            content = GetBadgeContent(badge);
            return content != null;
        }
        public bool HasBadge(BadgeDefinition badge, out BadgeContent content)
        {
            return HasBadgeInternal(badge, out content);
        }
        public void Init()
        {
            foreach (var badgeContent in m_Badges)
            {
                badgeContent.Init();
            }
        }
        public void UnlockBadge(BadgeDefinition badge)
        {
            if (HasBadge(badge, out var content) && !content.IsUnlocked)
            {
                content.UnlockBadge();
                m_OnBadgeUnlocked?.Invoke(content);
            }
        }
        public void AddCurrentUpgradeLevel(BadgeDefinition badge, int amount)
        {
            if (HasBadge(badge, out var content))
            {
                content.AddCurrentUpgradeLevel(amount);
                m_OnBadgeCurrentUpgradeLevelChanged?.Invoke(content);
            }
        }
        public void SetCurrentUpgradeLevel(BadgeDefinition badge, int level)
        {
            if (HasBadge(badge, out var content))
            {
                content.SetCurrentUpgradeLevel(level);
                m_OnBadgeCurrentUpgradeLevelChanged?.Invoke(content);
            }
        }
    }
    [System.Serializable]
    public class BadgeContent
    {
        [SerializeField]
        private BadgeDefinition m_Badge;
        [SerializeField]
        private bool m_IsUnlocked = false;
        [SerializeField]
        private int m_CurrentUpgradeLevel;
        [SerializeField]
        private UnityEvent m_OnUnlocked;
        public BadgeDefinition Badge => m_Badge;
        public bool IsUnlocked => m_IsUnlocked;
        public int CurrentUpgradeLevel => m_CurrentUpgradeLevel;

        private const string KEY_IS_UNLOCKED = "isunlocked";
        private const string KEY_CURRENT_UPGRADE_LEVEL = "currentupgradelevel";
        public void UnlockBadge()
        {
            m_IsUnlocked = true;
            m_OnUnlocked?.Invoke();
            UnityService.Instance.SaveData(KEY_IS_UNLOCKED, m_IsUnlocked);
        }
        private void OnValidate()
        {
            if (m_CurrentUpgradeLevel > m_Badge.Upgrade.Length)
            {
                m_CurrentUpgradeLevel = m_Badge.Upgrade.Length;
            }
            UnityService.Instance.SaveData(KEY_CURRENT_UPGRADE_LEVEL, m_CurrentUpgradeLevel);
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
            m_Badge = badge;
            m_IsUnlocked = false;
        }
        public void Init()
        {
            bool hasKeyUnlocked = UnityService.Instance.HasData(KEY_IS_UNLOCKED);
            bool hasKeyCurrentUpgradeLevel = UnityService.Instance.HasData(KEY_CURRENT_UPGRADE_LEVEL);
            if (hasKeyUnlocked)
            {
                m_IsUnlocked = UnityService.Instance.GetData<bool>(KEY_IS_UNLOCKED);
            }
            if (hasKeyCurrentUpgradeLevel)
            {
                m_CurrentUpgradeLevel = UnityService.Instance.GetData<int>(KEY_CURRENT_UPGRADE_LEVEL);
            }
        }
    }
}
