using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class BadgeView : UIView
    {
        private BadgeConfig m_Definition;

        [SerializeField]
        private Image m_Icon;
        [SerializeField]
        private Button m_DetailsButton;
        [SerializeField]
        private Button m_ClaimButton;

        private BadgeInfoPanel m_InfoPanel;
        private LootedPanel m_LootedPanel;
        public BadgeConfig Definition => m_Definition;

        private BadgeManager m_BadgeManager;
        private BadgeManager GetBadgeManager()
        {
            if (m_BadgeManager == null)
            {
                m_BadgeManager = Player.Instance.BadgeManager;
            }
            return m_BadgeManager;
        }
        private LootedPanel GetLootedPanel()
        {
            if (m_LootedPanel == null)
            {
                m_LootedPanel = CanvasManager.Instance.GetPanel<LootedPanel>();
            }
            return m_LootedPanel;
        }

        private void Start()
        {
            m_DetailsButton.onClick.RemoveAllListeners();
            m_DetailsButton.onClick.AddListener(ShowDetails);
            m_ClaimButton.onClick.RemoveAllListeners();
            m_ClaimButton.onClick.AddListener(ClaimReward);
        }
        private BadgeInfoPanel GetInfoPanel()
        {
            if (m_InfoPanel == null)
            {
                m_InfoPanel = CanvasManager.Instance.GetPanel<BadgeInfoPanel>();
            }
            return m_InfoPanel;
        }
        public void Init(BadgeConfig defi)
        {
            InitInternal(defi);
        }

        private void InitInternal(BadgeConfig defi)
        {
            m_Definition = defi;
            if (GetBadgeManager().HasBadge(defi, out BadgeContent content))
            {
                int currentLevel = content.CurrentUpgradeLevel;
                bool isUnlocked = content.IsUnlocked;
                bool canClaim = content.UnClaimedReward > 0;
                m_Icon.sprite = defi.Upgrade[currentLevel].Icon;
                m_Icon.color = isUnlocked ? Color.white : Color.gray;

                bool hasMaxLevel = currentLevel >= defi.Upgrade.Length - 1;
                m_ClaimButton.gameObject.SetActive(canClaim);
            }
            else
            {
                Debug.LogError($"BadgeManager does not have badge: {defi.name}");
            }
        }

        private void ShowDetails()
        {
            GetInfoPanel().Show();
            GetInfoPanel().Init(m_Definition);
        }

        private void ClaimReward()
        {
            if (GetBadgeManager().HasBadge(m_Definition, out BadgeContent content))
            {
                content.ClaimReward();
                GetLootedPanel().ShowLoot(BadgeHandler.RewardOnUnlock(content));
                Init(m_Definition);
            }
        }
    }
}
