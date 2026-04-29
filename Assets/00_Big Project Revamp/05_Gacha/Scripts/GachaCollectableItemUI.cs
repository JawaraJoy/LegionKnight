using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public abstract class GachaCollectableItemUI : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private TextMeshProUGUI m_RarityText;
        [SerializeField] private Image m_RarityFrame;

        // ── SetupBase overloads ───────────────────────────────────────────────

        // Full setup dari CollectibleConfig
        protected void SetupBase(CollectibleConfig collectible, int amount)
        {
            SetupBase(collectible.CollectibleField?.Icon,
                collectible.BaseInfo.Name, amount);
            RefreshRarityInternal(collectible.CollectibleField?.RarityConfig);
        }

        // Setup dari GachaCollectableConfig
        protected void SetupBase(GachaCollectableConfig collectable)
        {
            SetupBase(collectable.Collect, collectable.Amount);
        }

        // Bare minimum — hanya icon, name, amount
        // Dipakai saat flip agar tidak overwrite rarity
        protected void SetupBase(Sprite icon, string name, int amount)
        {
            if (m_Icon != null) m_Icon.sprite = icon;
            if (m_NameText != null) m_NameText.text = name;
            RefreshAmountInternal(amount);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

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

        protected virtual void OnSetupComplete(CollectibleConfig collectible, int amount) { }
    }
}