using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class ShopBundleContentUI : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private Image m_RarityFrame;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_AmountText;

        public void Setup(ShopBundleEntry entry)
        {
            if (m_Icon != null) m_Icon.sprite = entry.Collectible.CollectibleField?.Icon;
            if (m_NameText != null) m_NameText.text = entry.Collectible.BaseInfo.Name;
            if (m_RarityFrame != null)
            {
                m_RarityFrame.color = entry.Collectible.CollectibleField.RarityConfig.Color;
            }
            if (m_AmountText != null)
            {
                bool show = entry.Amount >= 2;
                m_AmountText.gameObject.SetActive(show);
                if (show) m_AmountText.text = $"x{entry.Amount}";
            }
        }
    }
}