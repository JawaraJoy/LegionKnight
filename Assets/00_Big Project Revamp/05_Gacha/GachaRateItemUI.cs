using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class GachaRateItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private TextMeshProUGUI m_ChanceText;
        [SerializeField] private Image m_Icon;

        public void Setup(GachaRateInfo rate)
        {
            if (m_NameText != null) m_NameText.text = rate.Collectable.Collect?.BaseInfo.Name ?? "-";
            if (m_AmountText != null) m_AmountText.text = $"x{rate.Collectable.Amount}";
            if (m_ChanceText != null) m_ChanceText.text = $"{rate.Percent:F2}%";
        }
    }
}