using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class ShopBundleItemUI : MonoBehaviour
    {
        [Header("Info")]
        [SerializeField] private Image m_BundleImage;
        [SerializeField] private TextMeshProUGUI m_BundleNameText;
        [SerializeField] private TextMeshProUGUI m_BundleDescText;

        [Header("Contents Preview")]
        [SerializeField] private Transform m_ContentsContainer;
        [SerializeField] private ShopBundleContentUI m_ContentItemPrefab;

        [Header("Price")]
        [SerializeField] private GameObject m_FreeLabel;
        [SerializeField] private GameObject m_PriceGroup;
        [SerializeField] private Image m_CurrencyIcon;
        [SerializeField] private TextMeshProUGUI m_OriginalPriceText;
        [SerializeField] private TextMeshProUGUI m_FinalPriceText;
        [SerializeField] private GameObject m_DiscountBadge;
        [SerializeField] private TextMeshProUGUI m_DiscountPercentText;

        [Header("Badges")]
        [SerializeField] private GameObject m_FirstPurchaseBadge;
        [SerializeField] private GameObject m_DailyBadge;

        [Header("Unavailable State")]
        [SerializeField] private GameObject m_UnavailableOverlay;
        [SerializeField] private TextMeshProUGUI m_CountdownText;
        [SerializeField] private TextMeshProUGUI m_UnavailableReasonText;

        [Header("Button")]
        [SerializeField] private Button m_BuyButton;
        [SerializeField] private TextMeshProUGUI m_BuyButtonText;

        private ShopBundleConfig m_Bundle;
        private Coroutine m_CountdownCoroutine;

        public void Setup(ShopBundleConfig bundle, ShopCostBreakdown breakdown,
            ShopBundleAvailability availability)
        {
            m_Bundle = bundle;

            RefreshInfoInternal(bundle);
            RefreshContentsInternal(bundle);
            RefreshPriceInternal(bundle, breakdown);
            RefreshAvailabilityInternal(availability);
            RefreshButtonInternal(breakdown, availability);
        }

        public void SetBuyListener(Action<ShopBundleConfig> onBuy)
        {
            if (m_BuyButton == null) return;
            m_BuyButton.onClick.RemoveAllListeners();
            m_BuyButton.onClick.AddListener(() => onBuy?.Invoke(m_Bundle));
        }

        private void OnDisable() => StopCountdownInternal();

        // ── Refresh ───────────────────────────────────────────────────────────

        private void RefreshInfoInternal(ShopBundleConfig bundle)
        {
            if (m_BundleImage != null) m_BundleImage.sprite = bundle.BundleSprite;
            if (m_BundleNameText != null) m_BundleNameText.text = bundle.BaseInfo.Name;
            if (m_BundleDescText != null) m_BundleDescText.text = bundle.BaseInfo.Description;
        }

        private void RefreshContentsInternal(ShopBundleConfig bundle)
        {
            if (m_ContentsContainer == null || m_ContentItemPrefab == null) return;
            foreach (Transform child in m_ContentsContainer) Destroy(child.gameObject);
            if (bundle.Entries == null) return;

            foreach (var entry in bundle.Entries)
            {
                var item = Instantiate(m_ContentItemPrefab, m_ContentsContainer);
                item.Setup(entry);
            }
        }

        private void RefreshPriceInternal(ShopBundleConfig bundle, ShopCostBreakdown breakdown)
        {
            bool isFree = breakdown.IsFree;
            bool hasDiscount = !isFree
                && breakdown.MainCurrencyAmount < breakdown.OriginalPrice;

            if (m_FreeLabel != null) m_FreeLabel.SetActive(isFree);
            if (m_PriceGroup != null) m_PriceGroup.SetActive(!isFree);

            if (!isFree)
            {
                if (m_CurrencyIcon != null)
                    m_CurrencyIcon.sprite = bundle.CostCurrency?.CollectibleField?.Icon;

                if (m_OriginalPriceText != null)
                {
                    m_OriginalPriceText.gameObject.SetActive(hasDiscount);
                    if (hasDiscount)
                        m_OriginalPriceText.text = $"<s>{breakdown.OriginalPrice}</s>";
                }

                if (m_FinalPriceText != null)
                    m_FinalPriceText.text = breakdown.MainCurrencyAmount.ToString();

                if (m_DiscountBadge != null)
                    m_DiscountBadge.SetActive(hasDiscount);

                if (m_DiscountPercentText != null && hasDiscount && breakdown.OriginalPrice > 0)
                {
                    float pct = (1f - (float)breakdown.MainCurrencyAmount
                                 / breakdown.OriginalPrice) * 100f;
                    m_DiscountPercentText.text = $"-{pct:F0}%";
                }
            }

            if (m_FirstPurchaseBadge != null)
                m_FirstPurchaseBadge.SetActive(breakdown.IsFirstPurchaseDiscount);

            if (m_DailyBadge != null)
                m_DailyBadge.SetActive(
                    bundle.PurchaseLimit == ShopBundlePurchaseLimit.Daily);
        }

        private void RefreshAvailabilityInternal(ShopBundleAvailability availability)
        {
            bool unavailable = !availability.CanPurchase;

            if (m_UnavailableOverlay != null)
                m_UnavailableOverlay.SetActive(unavailable);

            if (!unavailable)
            {
                StopCountdownInternal();
                return;
            }

            if (m_UnavailableReasonText != null)
            {
                m_UnavailableReasonText.text = availability.LimitType switch
                {
                    ShopBundlePurchaseLimit.OneTime => "Sudah dibeli",
                    ShopBundlePurchaseLimit.Daily => "Kembali besok",
                    _ => "Tidak tersedia"
                };
            }

            if (availability.IsDaily && availability.ResetSecondsRemaining > 0)
            {
                StopCountdownInternal();
                m_CountdownCoroutine = StartCoroutine(CountdownRoutine());
            }
            else if (m_CountdownText != null)
            {
                m_CountdownText.gameObject.SetActive(false);
            }
        }

        private void RefreshButtonInternal(ShopCostBreakdown breakdown,
            ShopBundleAvailability availability)
        {
            if (m_BuyButton == null) return;
            m_BuyButton.interactable = breakdown.CanAfford && availability.CanPurchase;

            if (m_BuyButtonText != null)
                m_BuyButtonText.text = breakdown.IsFree ? "Ambil" : "Beli";
        }

        // ── Countdown ─────────────────────────────────────────────────────────

        private IEnumerator CountdownRoutine()
        {
            if (m_CountdownText != null)
                m_CountdownText.gameObject.SetActive(true);

            while (true)
            {
                double remaining = RushPlayer.Instance.ShopManager
                    .GetAvailability(m_Bundle).ResetSecondsRemaining;

                if (remaining <= 0)
                {
                    if (m_CountdownText != null) m_CountdownText.gameObject.SetActive(false);
                    if (m_UnavailableOverlay != null) m_UnavailableOverlay.SetActive(false);
                    if (m_BuyButton != null) m_BuyButton.interactable = true;
                    m_CountdownCoroutine = null;
                    yield break;
                }

                if (m_CountdownText != null)
                    m_CountdownText.text = FormatCountdownInternal(remaining);

                yield return new WaitForSeconds(1f);
            }
        }

        private static string FormatCountdownInternal(double totalSeconds)
        {
            var span = TimeSpan.FromSeconds(totalSeconds);
            return span.Hours > 0
                ? $"{span.Hours:D2}:{span.Minutes:D2}:{span.Seconds:D2}"
                : $"{span.Minutes:D2}:{span.Seconds:D2}";
        }

        private void StopCountdownInternal()
        {
            if (m_CountdownCoroutine == null) return;
            StopCoroutine(m_CountdownCoroutine);
            m_CountdownCoroutine = null;
        }
    }
}