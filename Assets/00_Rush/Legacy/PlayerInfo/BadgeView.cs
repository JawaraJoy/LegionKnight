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

        [SerializeField] private TMPro.TMP_Text m_Label;
        [SerializeField] private TMPro.TMP_Text m_State;
        [SerializeField] private Image m_RarityFrame;
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
                var tier = defi.Upgrade[content.DisplayLevel];
                m_Icon.sprite = tier.Icon;
                m_Icon.color = content.IsUnlocked || content.CanClaim ? Color.white : Color.gray;
                m_ClaimButton.gameObject.SetActive(content.CanClaim);
                if (m_Label != null) m_Label.text = content.IsUnlocked || content.CanClaim ? tier.Label : defi.BaseInfo.Name;
                if (m_State != null) m_State.text = content.CanClaim ? "" : content.IsUnlocked ? "Earned" : "Locked";
                if (m_RarityFrame != null && defi.CollectibleField.RarityConfig != null)
                    m_RarityFrame.color = defi.CollectibleField.RarityConfig.Color;
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
            if (GetBadgeManager().TryClaimReward(m_Definition, out var rewards))
            {
                GetLootedPanel().ShowLoot(rewards);
                Init(m_Definition);
            }
        }
    }
}
