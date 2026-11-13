using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class IAPBundleItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_DescriptionText;
        [SerializeField] private Button m_Button;
        [SerializeField] private Image m_BundleImage;
        [SerializeField] private TextMeshProUGUI m_PriceText;
        [SerializeField] private GameObject m_FirstPurchaseBonusBadge;
        [SerializeField] private Image m_ItemIcon;
        [SerializeField] private Image m_RarityFrame;
        [SerializeField] private TMP_Text m_BonusLabel;
        private IAPBundleConfig m_Bundle;
        private Action<IAPBundleConfig> m_OnClicked;

        public void Setup(IAPBundleConfig bundle, string localizedPrice, bool isFirstPurchase,
            bool canPurchase, Action<IAPBundleConfig> onClicked)
        {
            m_Bundle = bundle;
            m_OnClicked = onClicked;
            var entries = bundle.Entries;
            var first = entries is { Length: > 0 } ? entries[0] : null;
            var collectible = first?.Collectible;
            if (m_NameText != null) m_NameText.text = bundle.BaseInfo.Name;
            if (m_DescriptionText != null) m_DescriptionText.text = first == null ? bundle.BaseInfo.Description :
                first.Amount.ToString("N0", System.Globalization.CultureInfo.InvariantCulture) +
                (entries.Length > 1 ? $"  +{entries.Length - 1} more" : "");
            if (m_BundleImage != null) m_BundleImage.sprite = bundle.BundleSprite;
            if (m_ItemIcon != null) { m_ItemIcon.sprite = collectible?.CollectibleField.Icon; m_ItemIcon.enabled = m_ItemIcon.sprite != null; }
            if (m_RarityFrame != null) m_RarityFrame.color = collectible?.CollectibleField.RarityConfig?.Color ?? Color.white;
            if (m_PriceText != null) m_PriceText.text = !canPurchase ? "Sold out" :
                string.IsNullOrWhiteSpace(localizedPrice) ? "View details" : localizedPrice;
            if (m_FirstPurchaseBonusBadge != null)
                m_FirstPurchaseBonusBadge.SetActive(isFirstPurchase && bundle.HasFirstPurchaseBonus);
            if (m_BonusLabel != null && isFirstPurchase && bundle.HasFirstPurchaseBonus)
            {
                var bonus = bundle.FirstPurchaseBonusEntries;
                bool doubled = entries?.Length == 1 && bonus.Length == 1 && first != null &&
                    bonus[0] != null && bonus[0].Collectible == collectible && bonus[0].Amount == first.Amount;
                m_BonusLabel.text = doubled ? "First purchase 2x" : "First purchase bonus";
            }
            if (m_Button == null) return;
            m_Button.onClick.RemoveListener(OnClickedInternal);
            m_Button.onClick.AddListener(OnClickedInternal);
        }

        private void OnClickedInternal() => m_OnClicked?.Invoke(m_Bundle);
    }
}
