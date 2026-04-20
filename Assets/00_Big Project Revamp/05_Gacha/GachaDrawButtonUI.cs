using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class GachaDrawButtonUI : MonoBehaviour
    {
        [SerializeField] private Button m_Button;

        [Header("Main Currency Row")]
        [SerializeField] private GameObject m_MainCostRow;
        [SerializeField] private Image m_MainCurrencyIcon;
        [SerializeField] private TextMeshProUGUI m_OriginalCostText;  // dicoret jika ada discount
        [SerializeField] private TextMeshProUGUI m_MainCostText;      // sisa main atau total cost

        [Header("Discount")]
        [SerializeField] private GameObject m_DiscountBadge;
        [SerializeField] private TextMeshProUGUI m_DiscountPercentText;

        [Header("Alt Currency Row — tampil jika main tidak cukup")]
        [SerializeField] private GameObject m_AltCostRow;
        [SerializeField] private Image m_AltCurrencyIcon;
        [SerializeField] private TextMeshProUGUI m_AltCostText;

        public Button Button => m_Button;

        public void Refresh(GachaCostBreakdown breakdown, GachaBannerConfig banner, int mainHeld)
        {
            RefreshCostRowsInternal(breakdown, banner, mainHeld);
            RefreshDiscountInternal(breakdown);

            if (m_Button != null) m_Button.interactable = breakdown.CanAfford;
        }

        private void RefreshCostRowsInternal(GachaCostBreakdown breakdown,
            GachaBannerConfig banner, int mainHeld)
        {
            bool mainIsEmpty = mainHeld <= 0;
            bool mainNotEnough = mainHeld < breakdown.TotalCost;
            bool hasAlt = breakdown.AltDeductAmount > 0
                                  && banner.AltCostCurrency != null;

            // ── Main row ──────────────────────────────────────────────────────
            // Sembunyikan main row hanya jika main benar-benar kosong DAN ada alt
            bool showMain = !(mainIsEmpty && hasAlt);
            if (m_MainCostRow != null) m_MainCostRow.SetActive(showMain);

            if (showMain)
            {
                if (m_MainCurrencyIcon != null)
                    m_MainCurrencyIcon.sprite = banner.DrawCostCurrency?.CollectibleField?.Icon;

                if (m_MainCostText != null)
                {
                    // Main cukup → tampilkan total cost (dari config)
                    // Main kurang → tampilkan sisa main yang player punya
                    m_MainCostText.text = mainNotEnough
                        ? mainHeld.ToString()
                        : breakdown.TotalCost.ToString();
                }

                // Original cost dicoret — hanya jika ada discount DAN main cukup
                // (jika main kurang, kita sudah tampilkan sisa, bukan cost asli)
                if (m_OriginalCostText != null)
                {
                    bool showOriginal = breakdown.HasDiscount && !mainNotEnough;
                    m_OriginalCostText.gameObject.SetActive(showOriginal);
                    if (showOriginal)
                        m_OriginalCostText.text = $"<s>{breakdown.OriginalCost}</s>";
                }
            }

            // ── Alt row ───────────────────────────────────────────────────────
            // Tampil jika main tidak cukup dan ada alt yang dibutuhkan
            bool showAlt = mainNotEnough && hasAlt;
            if (m_AltCostRow != null) m_AltCostRow.SetActive(showAlt);

            if (showAlt)
            {
                if (m_AltCurrencyIcon != null)
                    m_AltCurrencyIcon.sprite = banner.AltCostCurrency?.CollectibleField?.Icon;

                if (m_AltCostText != null)
                    m_AltCostText.text = breakdown.AltDeductAmount.ToString();
            }
        }

        private void RefreshDiscountInternal(GachaCostBreakdown breakdown)
        {
            if (m_DiscountBadge != null)
                m_DiscountBadge.SetActive(breakdown.HasDiscount);

            if (m_DiscountPercentText != null && breakdown.HasDiscount
                && breakdown.OriginalCost > 0)
            {
                float pct = (1f - (float)breakdown.TotalCost / breakdown.OriginalCost) * 100f;
                m_DiscountPercentText.text = $"-{pct:F0}%";
            }
        }

    }
}