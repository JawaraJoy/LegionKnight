using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class IAPBundleItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_NameText;

        [Header("Visual")]
        [SerializeField] private Button m_Button;
        [SerializeField] private Image m_BundleImage;

        [Header("Price")]
        // Localized price string from the store (e.g. "$0.99" / "Rp 15.000")
        [SerializeField] private TextMeshProUGUI m_PriceText;

        [Header("First Purchase Bonus Badge")]
        [SerializeField] private GameObject m_FirstPurchaseBonusBadge;

        private IAPBundleConfig m_Bundle;

        public void Setup(IAPBundleConfig bundle, string localizedPrice,
            bool isFirstPurchase, bool canPurchase, Action<IAPBundleConfig> onClicked)
        {
            m_Bundle = bundle;

            if (m_NameText != null) m_NameText.text = bundle.BaseInfo.Name;
            if (m_BundleImage != null) m_BundleImage.sprite = bundle.BundleSprite;
            if (m_PriceText != null) m_PriceText.text = localizedPrice;

            if (m_FirstPurchaseBonusBadge != null)
                m_FirstPurchaseBonusBadge.SetActive(
                    isFirstPurchase && bundle.HasFirstPurchaseBonus);

            if (m_Button != null)
            {
                m_Button.interactable = canPurchase;
                m_Button.onClick.RemoveAllListeners();
                if (canPurchase)
                    m_Button.onClick.AddListener(() => onClicked?.Invoke(m_Bundle));
            }
        }
    }
}