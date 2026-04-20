using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class ShopBundleItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_NameText;
        [Header("Visual")]
        [SerializeField] private Button m_Button;
        [SerializeField] private Image m_BundleImage;

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
        [SerializeField] private TextMeshProUGUI m_UnavailableReasonText;
        [SerializeField] private TextMeshProUGUI m_CountdownText;

        private ShopBundleConfig m_Bundle;
        private Coroutine m_CountdownCoroutine;

        public void Setup(ShopBundleConfig bundle, ShopCostBreakdown breakdown,
            ShopBundleAvailability availability, Action<ShopBundleConfig> onClicked)
        {
            m_Bundle = bundle;
            m_NameText.text = m_Bundle.BaseInfo.Name;
            if (m_BundleImage != null)
                m_BundleImage.sprite = bundle.BundleSprite;

            RefreshPriceInternal(bundle, breakdown);
            RefreshBadgesInternal(bundle, breakdown, availability);
            RefreshAvailabilityInternal(availability);

            if (m_Button != null)
            {
                m_Button.onClick.RemoveAllListeners();
                m_Button.onClick.AddListener(() => onClicked?.Invoke(m_Bundle));
            }
        }

        private void OnDisable() => StopCountdownInternal();

        // ── Price ─────────────────────────────────────────────────────────────

        private void RefreshPriceInternal(ShopBundleConfig bundle, ShopCostBreakdown breakdown)
        {
            bool isFree = breakdown.IsFree;
            bool hasDiscount = !isFree
                && breakdown.MainCurrencyAmount < breakdown.OriginalPrice;

            if (m_FreeLabel != null) m_FreeLabel.SetActive(isFree);
            if (m_PriceGroup != null) m_PriceGroup.SetActive(!isFree);

            if (isFree) return;

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

        // ── Badges ────────────────────────────────────────────────────────────

        private void RefreshBadgesInternal(ShopBundleConfig bundle,
            ShopCostBreakdown breakdown, ShopBundleAvailability availability)
        {
            if (m_FirstPurchaseBadge != null)
                m_FirstPurchaseBadge.SetActive(breakdown.IsFirstPurchaseDiscount);

            if (m_DailyBadge != null)
                m_DailyBadge.SetActive(
                    bundle.PurchaseLimit == ShopBundlePurchaseLimit.Daily);
        }

        // ── Availability ──────────────────────────────────────────────────────

        private void RefreshAvailabilityInternal(ShopBundleAvailability availability)
        {
            bool unavailable = !availability.CanPurchase;

            if (m_UnavailableOverlay != null)
                m_UnavailableOverlay.SetActive(unavailable);

            if (!unavailable)
            {
                StopCountdownInternal();
                if (m_CountdownText != null)
                    m_CountdownText.gameObject.SetActive(false);
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
                    if (m_Button != null) m_Button.interactable = true;
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
            var span = System.TimeSpan.FromSeconds(totalSeconds);
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