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
                var tier = defi.Upgrade[content.DisplayLevel];
                bool visible = content.IsUnlocked || content.CanClaim;
                m_Icon.sprite = tier.Icon;
                m_NameText.text = visible ? tier.Label : defi.BaseInfo.Name;
                m_DescriptionText.text = visible ? tier.Description : defi.BaseInfo.Description;
                if (content.CanClaim)
                {
                    string reward = string.Join(", ", System.Array.ConvertAll(tier.RewardOnUnlock,
                        row => row.Amount + " " + row.ItemLoot.BaseInfo.Name));
                    m_DescriptionText.text = "Ready to claim: " + reward;
                }
                m_Icon.color = visible ? Color.white : Color.gray;
                
            }
            else
            {
                Debug.LogError($"BadgeManager does not have badge: {defi.name}");
            }
        }
    }
}
