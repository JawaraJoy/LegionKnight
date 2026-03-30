// ─────────────────────────────────────────────────────────────────────────────
// Concrete inventory panels dan widgets.
// File ini berisi semua implementasi konkret yang tipis karena logika
// sudah ada di base class.
// ─────────────────────────────────────────────────────────────────────────────

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    // ══════════════════════════════════════════════════════════════════════════
    // HERO
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Panel grid semua hero (locked + owned).
    /// Refresh dipanggil dari luar setelah HeroInventory berubah.
    /// </summary>
    public class HeroInventoryPanel : CollectiblePanelView<HeroInventoryEntry, HeroWidget>
    {
        // Tampilkan SEMUA hero — locked maupun owned (default behavior dari base class)
        // Tidak perlu override ShouldShowEntry.

        protected override void OnEntrySelected(HeroInventoryEntry entry)
        {
            // Contoh: enable tombol "Select" hanya jika hero sudah dimiliki
            // if (m_SelectButton) m_SelectButton.interactable = entry.IsOwned;
        }
    }

    /// <summary>Card di grid hero: menampilkan stars di bawah icon.</summary>
    public class HeroWidget : CollectibleWidget<HeroInventoryEntry>
    {
        [SerializeField] private Transform  m_StarsContainer;
        [SerializeField] private GameObject m_StarFilledPrefab;
        [SerializeField] private GameObject m_StarEmptyPrefab;

        protected override void OnBind(HeroInventoryEntry entry)
        {
            if (!m_StarsContainer) return;

            foreach (Transform child in m_StarsContainer)
                Destroy(child.gameObject);

            for (int i = 0; i < entry.MaxStars; i++)
            {
                GameObject prefab = i < entry.CurrentStars ? m_StarFilledPrefab : m_StarEmptyPrefab;
                if (prefab) Instantiate(prefab, m_StarsContainer);
            }
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // CARD
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Panel grid semua card (locked + owned).
    /// </summary>
    public class CardInventoryPanel : CollectiblePanelView<CardInventoryEntry, CardWidget>
    {
        // Tampilkan SEMUA card — sama seperti hero, tidak perlu override.
    }

    /// <summary>Card di grid card: tidak ada info tambahan selain base (icon, nama, locked).</summary>
    public class CardWidget : CollectibleWidget<CardInventoryEntry>
    {
        // Base class sudah cukup untuk card widget.
        // Tambahkan komponen tambahan di sini jika dibutuhkan (misal: jumlah skill).
    }

    // ══════════════════════════════════════════════════════════════════════════
    // ITEM
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Panel grid item milik player.
    /// Hanya menampilkan item dengan Quantity > 0 (sesuai requirement UI).
    /// </summary>
    public class ItemInventoryPanel : CollectiblePanelView<ItemInventoryEntry, ItemWidget>
    {
        /// <summary>Item hanya tampil jika player punya minimal 1.</summary>
        protected override bool ShouldShowEntry(ItemInventoryEntry entry)
            => entry.IsVisible; // IsVisible == Quantity > 0
    }

    /// <summary>Card di grid item: menampilkan jumlah (quantity) di sudut icon.</summary>
    public class ItemWidget : CollectibleWidget<ItemInventoryEntry>
    {
        [SerializeField] private TextMeshProUGUI m_QuantityText;
        [SerializeField] private GameObject      m_UniqueBadge;

        protected override void OnBind(ItemInventoryEntry entry)
        {
            if (m_QuantityText)
                m_QuantityText.text = entry.IsUnique ? "1" : $"x{entry.Quantity}";

            if (m_UniqueBadge)
                m_UniqueBadge.SetActive(entry.IsUnique);
        }
    }
}
