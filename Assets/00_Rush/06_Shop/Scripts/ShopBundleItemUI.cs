using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class ShopBundleItemUI : MonoBehaviour, IUpdater
    {
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private Button m_Button;
        [SerializeField] private Image m_BundleImage;
        [SerializeField] private ShopBundleArtwork m_Artwork;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private GameObject m_FreeLabel;
        [SerializeField] private GameObject m_PriceGroup;
        [SerializeField] private Image m_CurrencyIcon;
        [SerializeField] private GameObject m_OriginalCostContent;
        [SerializeField] private StrikethroughText m_OriginalPriceText;
        [SerializeField] private TextMeshProUGUI m_FinalPriceText;
        [SerializeField] private GameObject m_DiscountBadge;
        [SerializeField] private TextMeshProUGUI m_DiscountPercentText;
        [SerializeField] private GameObject m_FirstPurchaseBadge;
        [SerializeField] private GameObject m_DailyBadge;
        [SerializeField] private GameObject m_UnavailableOverlay;
        [SerializeField] private TextMeshProUGUI m_UnavailableReasonText;
        [SerializeField] private TextMeshProUGUI m_CountdownText;

        private const float c_TickSeconds = 1f;
        private ShopBundleConfig m_Bundle;
        private Action<ShopBundleConfig> m_OnClicked;
        private bool m_CountingDown;
        private float m_NextTick;
        private UpdateBank m_UpdateBank;
        public bool IsActive => m_CountingDown && Time.unscaledTime >= m_NextTick;

        private void Awake()
        {
            if (m_Button != null) m_Button.onClick.AddListener(OnButtonClickedInternal);
        }

        private void OnEnable()
        {
            m_UpdateBank = UpdateBank.Instance;
            if (m_UpdateBank != null) m_UpdateBank.RegisterUpdateTick(gameObject, this);
            m_NextTick = 0f;
        }

        private void OnDisable()
        {
            if (m_UpdateBank != null) m_UpdateBank.UnregisterUpdateTick(gameObject);
        }

        public void Tick()
        {
            m_NextTick = Time.unscaledTime + c_TickSeconds;
            RefreshAvailabilityInternal(RushPlayer.Instance.ShopManager.GetAvailability(m_Bundle));
        }

        public void Setup(ShopBundleConfig bundle, ShopCostBreakdown price,
            ShopBundleAvailability availability, Action<ShopBundleConfig> onClicked)
        {
            m_Bundle = bundle;
            m_OnClicked = onClicked;
            if (m_NameText != null) m_NameText.text = bundle.BaseInfo.Name;
            if (m_BundleImage != null) m_BundleImage.sprite = bundle.BundleSprite;
            if (m_Artwork != null) m_Artwork.Bind(bundle);
            if (m_AmountText != null) m_AmountText.text = bundle.ContentsLabel;
            bool discount = price.HasDiscount;
            if (m_FreeLabel != null) m_FreeLabel.SetActive(price.IsFree);
            if (m_PriceGroup != null) m_PriceGroup.SetActive(!price.IsFree);
            if (m_CurrencyIcon != null) m_CurrencyIcon.sprite = bundle.CostCurrency?.CollectibleField?.Icon;
            if (m_OriginalCostContent != null) m_OriginalCostContent.SetActive(discount);
            if (m_OriginalPriceText != null && discount) m_OriginalPriceText.SetText(price.OriginalPrice.ToString());
            if (m_FinalPriceText != null) m_FinalPriceText.text = price.MainCurrencyAmount.ToString();
            if (m_DiscountBadge != null) m_DiscountBadge.SetActive(discount);
            if (m_DiscountPercentText != null && discount) m_DiscountPercentText.text = price.DiscountLabel;
            if (m_FirstPurchaseBadge != null) m_FirstPurchaseBadge.SetActive(price.IsFirstPurchaseDiscount);
            if (m_DailyBadge != null) m_DailyBadge.SetActive(availability.IsDaily);
            RefreshAvailabilityInternal(availability);
        }

        private void RefreshAvailabilityInternal(ShopBundleAvailability availability)
        {
            if (m_Button != null) m_Button.interactable = availability.CanPurchase;
            if (m_UnavailableOverlay != null) m_UnavailableOverlay.SetActive(!availability.CanPurchase);
            if (m_UnavailableReasonText != null && !availability.CanPurchase)
                m_UnavailableReasonText.text = availability.IsDaily ? "Claimed today" : "Already purchased";
            m_CountingDown = !availability.CanPurchase && availability.IsDaily;
            if (m_CountdownText == null) return;
            m_CountdownText.gameObject.SetActive(m_CountingDown);
            if (m_CountingDown)
            {
                var time = TimeSpan.FromSeconds(Math.Ceiling(availability.ResetSecondsRemaining));
                m_CountdownText.text = $"{(int)time.TotalHours:00}:{time.Minutes:00}:{time.Seconds:00}";
            }
        }

        private void OnButtonClickedInternal()
        {
            if (m_Button.interactable) m_OnClicked?.Invoke(m_Bundle);
        }
    }
}
