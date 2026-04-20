using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class GachaDrawButtonUI : MonoBehaviour
    {
        [SerializeField] private Button m_Button;

        [Header("Main Currency Cost")]
        [SerializeField] private GameObject m_MainCostRow;
        [SerializeField] private Image m_MainCurrencyIcon;
        [SerializeField] private StrikethroughText m_OriginalCostText;
        [SerializeField] private TextMeshProUGUI m_FinalCostText;
        [SerializeField] private GameObject m_DiscountBadge;
        [SerializeField] private TextMeshProUGUI m_DiscountPercentText;

        [Header("Alt Currency — visible if main is insufficient")]
        [SerializeField] private GameObject m_AltCostRow;
        [SerializeField] private Image m_AltCurrencyIcon;
        [SerializeField] private TextMeshProUGUI m_AltCostText;

        public Button Button => m_Button;

        public void Refresh(GachaCostBreakdown breakdown, GachaBannerConfig banner, int mainHeld)
        {
            RefreshMainCostInternal(breakdown, banner, mainHeld);
            RefreshAltCostInternal(breakdown, banner, mainHeld);

            if (m_Button != null) m_Button.interactable = breakdown.CanAfford;
        }

        private void RefreshMainCostInternal(GachaCostBreakdown breakdown,
            GachaBannerConfig banner, int mainHeld)
        {
            bool mainIsEmpty = mainHeld <= 0;
            bool mainNotEnough = mainHeld < breakdown.TotalCost;

            // Hide entire main row if player has zero main currency
            if (m_MainCostRow != null) m_MainCostRow.SetActive(!mainIsEmpty);
            if (mainIsEmpty) return;

            if (m_MainCurrencyIcon != null)
                m_MainCurrencyIcon.sprite = banner.DrawCostCurrency?.CollectibleField?.Icon;

            // Show strikethrough original cost only if discounted and main is sufficient
            if (m_OriginalCostText != null)
            {
                bool showOriginal = breakdown.HasDiscount && !mainNotEnough;
                m_OriginalCostText.SetVisible(showOriginal);
                if (showOriginal)
                    m_OriginalCostText.SetText(breakdown.OriginalCost.ToString());
            }

            if (m_FinalCostText != null)
                m_FinalCostText.text = mainNotEnough
                    ? mainHeld.ToString()
                    : breakdown.TotalCost.ToString();

            if (m_DiscountBadge != null)
                m_DiscountBadge.SetActive(breakdown.HasDiscount);

            if (m_DiscountPercentText != null && breakdown.HasDiscount
                && breakdown.OriginalCost > 0)
            {
                float pct = (1f - (float)breakdown.TotalCost / breakdown.OriginalCost) * 100f;
                m_DiscountPercentText.text = $"-{pct:F0}%";
            }
        }

        private void RefreshAltCostInternal(GachaCostBreakdown breakdown,
            GachaBannerConfig banner, int mainHeld)
        {
            bool showAlt = mainHeld < breakdown.TotalCost
                           && breakdown.AltDeductAmount > 0
                           && banner.AltCostCurrency != null;

            if (m_AltCostRow != null) m_AltCostRow.SetActive(showAlt);
            if (!showAlt) return;

            if (m_AltCurrencyIcon != null)
                m_AltCurrencyIcon.sprite = banner.AltCostCurrency?.CollectibleField?.Icon;

            if (m_AltCostText != null)
                m_AltCostText.text = breakdown.AltDeductAmount.ToString();
        }
    }
}