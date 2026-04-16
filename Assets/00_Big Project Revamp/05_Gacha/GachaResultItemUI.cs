using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class GachaResultItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_AmountText;

        public void Setup(GachaCollectableConfig collectable)
        {
            if (m_NameText != null) m_NameText.text = collectable.Collect?.BaseInfo.Name ?? "-";
            if (m_AmountText != null) m_AmountText.text = $"x{collectable.Amount}";
        }
    }
}