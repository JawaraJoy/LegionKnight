using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class BadgeInfoPanel : PanelView
    {
        [SerializeField]
        private Image m_Icon;
        [SerializeField]
        private TextMeshProUGUI m_NameText;
        [SerializeField]
        private TextMeshProUGUI m_DescriptionText;

        private BadgeConfig m_Definition;

        public void Init(BadgeConfig defi)
        {
            m_Definition = defi;
            if (Player.Instance.BadgeManager.HasBadge(defi, out var content))
            {
                bool isUnlocked = content.IsUnlocked;
                int currentLevel = content.CurrentUpgradeLevel;
                if (content.IsUnlocked)
                {
                    m_Icon.sprite = defi.Upgrade[currentLevel].Icon;
                    m_NameText.text = defi.Upgrade[currentLevel].Label;
                    m_DescriptionText.text = defi.Upgrade[currentLevel].Description;
                }
                else
                {
                    m_Icon.sprite = defi.Upgrade[0].Icon;
                    m_NameText.text = defi.BaseInfo.Name;
                    m_DescriptionText.text = defi.BaseInfo.Description;
                }
                m_Icon.color = isUnlocked ? Color.white : Color.gray;
                
            }
            else
            {
                Debug.LogError($"BadgeManager does not have badge: {defi.name}");
            }
        }
    }
}
