using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    // Base class untuk semua UI yang menampilkan satu GachaCollectableConfig
    // GachaRateItemUI dan GachaResultItemUI mewarisi ini
    public abstract class GachaCollectableItemUI : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private TextMeshProUGUI m_RarityText;
        [SerializeField] private Image m_RarityFrame;

        // Subclass memanggil ini sebagai base setup
        protected void SetupBase(GachaCollectableConfig collectable)
        {
            if (m_Icon != null) m_Icon.sprite = collectable.Collect.CollectibleField.Icon;
            if (m_NameText != null) m_NameText.text = collectable.Collect?.BaseInfo.Name ?? "-";

            RefreshAmountInternal(collectable.Amount);
            RefreshRarityInternal(collectable.Collect.CollectibleField.RarityConfig);
        }

        private void RefreshAmountInternal(int amount)
        {
            if (m_AmountText == null) return;
            bool show = amount >= 2;
            m_AmountText.gameObject.SetActive(show);
            if (show) m_AmountText.text = $"x{amount}";
        }

        private void RefreshRarityInternal(RarityConfig rarity)
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
                m_RarityFrame.gameObject.SetActive(true);
                m_RarityFrame.color = rarity.Color;
            }
        }

        // Hook opsional untuk subclass jika perlu menambah setup setelah base
        protected virtual void OnSetupComplete(GachaCollectableConfig collectable) { }
    }
}