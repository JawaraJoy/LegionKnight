using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class GachaResultItemUI : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private TextMeshProUGUI m_RarityText;
        [SerializeField] private Image m_RarityFrame;

        public void Setup(GachaCollectableConfig collectable)
        {
            if (m_Icon != null) m_Icon.sprite = collectable.Collect.CollectibleField.Icon;
            if (m_NameText != null) m_NameText.text = collectable.Collect?.BaseInfo.Name ?? "-";

            // Sembunyikan amount jika kurang dari 2
            if (m_AmountText != null)
            {
                bool showAmount = collectable.Amount >= 2;
                m_AmountText.gameObject.SetActive(showAmount);
                if (showAmount) m_AmountText.text = $"x{collectable.Amount}";
            }

            RefreshRarityDisplayInternal(collectable.Collect.CollectibleField.RarityConfig);
        }

        private void RefreshRarityDisplayInternal(RarityConfig rarity)
        {
            if (rarity == null)
            {
                if (m_RarityText != null) m_RarityText.gameObject.SetActive(false);
                if (m_RarityFrame != null) m_RarityFrame.gameObject.SetActive(false);
                return;
            }

            if (m_RarityText != null)
            {
                m_RarityText.gameObject.SetActive(true);
                m_RarityText.text = rarity.BaseInfo.Name ?? "-";
                m_RarityText.color = rarity.Color;
            }

            if (m_RarityFrame != null)
            {
                m_RarityFrame.color = rarity.Color;
            }
        }
    }
}