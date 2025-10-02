using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class BadgeView : UIView
    {
        private BadgeDefinition m_Definition;

        [SerializeField]
        private Image m_Icon;
        [SerializeField]
        private Button m_DetailsButton;

        private BadgeInfoPanel m_InfoPanel;
        public BadgeDefinition Definition => m_Definition;

        private BadgeManager m_BadgeManager;
        private BadgeManager GetBadgeManager()
        {
            if (m_BadgeManager == null)
            {
                m_BadgeManager = Player.Instance.BadgeManager;
            }
            return m_BadgeManager;
        }

        private void Start()
        {
            m_DetailsButton.onClick.RemoveAllListeners();
            m_DetailsButton.onClick.AddListener(ShowDetails);
        }
        private BadgeInfoPanel GetInfoPanel()
        {
            if (m_InfoPanel == null)
            {
                m_InfoPanel = GameManager.Instance.GetPanel<BadgeInfoPanel>();
            }
            return m_InfoPanel;
        }
        public void Init(BadgeDefinition defi)
        {
            m_Definition = defi;
            if (GetBadgeManager().HasBadge(defi, out var content))
            {
                int currentLevel = content.CurrentUpgradeLevel;
                bool isUnlocked = content.IsUnlocked;
                m_Icon.sprite = defi.Upgrade[currentLevel].Icon;
                m_Icon.color = isUnlocked ? Color.white : Color.gray;
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
    }
}
