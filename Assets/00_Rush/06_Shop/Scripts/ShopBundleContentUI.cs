using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class ShopBundleContentUI : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private Image m_Frame;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_AmountText;

        [SerializeField] private TextMeshProUGUI m_RarityText;
        [SerializeField] private bool m_ShowSingleAmount;

        public void Setup(ShopBundleEntry entry)
        {
            SetupInternal(entry.Collectible, entry.Amount);
        }

        public void SetupFromIAP(IAPBundleEntry entry)
        {
            SetupInternal(entry.Collectible, entry.Amount);
        }

        private void SetupInternal(CollectibleConfig collectible, int amount)
        {
            if (collectible == null) return;
            var rarity = collectible.CollectibleField?.RarityConfig;
            if (m_RarityText != null) m_RarityText.text = rarity?.BaseInfo.Name?.ToUpperInvariant() ?? string.Empty;
            if (m_Icon != null) m_Icon.sprite = collectible.CollectibleField?.Icon;
            if (m_Frame != null) m_Frame.color = rarity?.Color ?? Color.white;
            if (m_NameText != null) m_NameText.text = collectible.BaseInfo.Name;

            if (m_AmountText != null)
            {
                bool show = m_ShowSingleAmount || amount >= 2;
                m_AmountText.gameObject.SetActive(show);
                if (show) m_AmountText.text = $"x{amount}";
            }
        }
    }
}
