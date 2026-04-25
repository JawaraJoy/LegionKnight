using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    // Displays a single gacha result during the preview sequence
    // Shows splash image, rarity frame, name — no pool needed, this is a single reused view
    public class GachaPreviewItemUI : MonoBehaviour
    {
        [Header("Display")]
        [SerializeField] private Image m_SplashImage;
        [SerializeField] private Image m_RarityFrame;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private TextMeshProUGUI m_RarityText;

        public void Setup(CollectibleResultEntry entry)
        {
            var collectible = entry.Collectible;
            var field = collectible.CollectibleField;

            if (m_SplashImage != null) m_SplashImage.sprite = field?.SplashImage;
            if (m_NameText != null) m_NameText.text = collectible.BaseInfo.Name;

            if (m_AmountText != null)
            {
                bool show = entry.Amount >= 2;
                m_AmountText.gameObject.SetActive(show);
                if (show) m_AmountText.text = $"x{entry.Amount}";
            }

            if (field?.RarityConfig != null)
            {
                if (m_RarityText != null)
                {
                    m_RarityText.gameObject.SetActive(true);
                    m_RarityText.text = field.RarityConfig.BaseInfo.Name;
                    m_RarityText.color = field.RarityConfig.Color;
                }
                if (m_RarityFrame != null)
                {
                    m_RarityFrame.gameObject.SetActive(true);
                    m_RarityFrame.color = field.RarityConfig.Color;
                }
            }
            else
            {
                if (m_RarityText != null) m_RarityText.gameObject.SetActive(false);
                if (m_RarityFrame != null) m_RarityFrame.gameObject.SetActive(false);
            }
        }
    }
}