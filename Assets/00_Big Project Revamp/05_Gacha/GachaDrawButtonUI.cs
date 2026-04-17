using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    // Attach ke GameObject yang sama dengan Button draw (single atau multi)
    // GachaPanel cukup memanggil Refresh() saat data berubah
    public class GachaDrawButtonUI : MonoBehaviour
    {
        [SerializeField] private Button m_Button;

        // Icon currency yang dipakai untuk bayar
        [SerializeField] private Image m_CurrencyIcon;

        // Cost tanpa discount — dicoret jika ada discount aktif
        [SerializeField] private TextMeshProUGUI m_OriginalCostText;

        // Cost setelah discount — selalu tampil
        [SerializeField] private TextMeshProUGUI m_FinalCostText;

        // Opsional: badge "DISKON" atau persentase diskon
        [SerializeField] private GameObject m_DiscountBadge;
        [SerializeField] private TextMeshProUGUI m_DiscountPercentText;

        public Button Button => m_Button;

        // originalCost  = biaya sebelum discount (atau sama jika tidak ada discount)
        // finalCost     = biaya setelah discount
        // currencyIcon  = sprite dari ItemConfig currency
        public void Refresh(int originalCost, int finalCost, Sprite currencyIcon)
        {
            bool hasDiscount = finalCost < originalCost;

            if (m_CurrencyIcon != null)
            {
                m_CurrencyIcon.sprite = currencyIcon;
                m_CurrencyIcon.gameObject.SetActive(currencyIcon != null);
            }

            // Original cost: tampil hanya jika ada discount, dengan strikethrough via
            // TextMeshPro rich text <s>
            if (m_OriginalCostText != null)
            {
                m_OriginalCostText.gameObject.SetActive(hasDiscount);
                if (hasDiscount)
                    m_OriginalCostText.text = $"<s>{originalCost}</s>";
            }

            if (m_FinalCostText != null)
                m_FinalCostText.text = finalCost.ToString();

            // Badge diskon
            if (m_DiscountBadge != null)
                m_DiscountBadge.SetActive(hasDiscount);

            if (m_DiscountPercentText != null && hasDiscount && originalCost > 0)
            {
                float percent = (1f - (float)finalCost / originalCost) * 100f;
                m_DiscountPercentText.text = $"-{percent:F0}%";
            }
        }

        public void SetInteractable(bool interactable)
        {
            if (m_Button != null) m_Button.interactable = interactable;
        }
    }
}