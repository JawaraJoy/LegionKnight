using TMPro;
using UnityEngine;

namespace Rush
{
    /// <summary>
    /// Section yang menampilkan detail khusus item:
    /// jumlah yang dimiliki, stack limit, badge unique.
    ///
    /// Hanya tampil ketika entry yang di-bind adalah ItemInventoryEntry.
    /// </summary>
    public class ItemDetailSection : CollectibleDetailSection
    {
        [Header("Quantity")]
        [SerializeField] private TextMeshProUGUI m_QuantityText;
        [SerializeField] private TextMeshProUGUI m_MaxStackText;

        [Header("Badges")]
        [SerializeField] private GameObject m_UniqueBadge;

        // ── CollectibleDetailSection ──────────────────────────────────
        public override bool IsRelevantFor(ICollectibleEntry entry)
        {
            return entry is ItemInventoryEntry;
        }

        protected override void OnBind(ICollectibleEntry entry)
        {
            if (entry is not ItemInventoryEntry itemEntry) return;

            if (m_QuantityText) m_QuantityText.text = $"x{itemEntry.Quantity}";
            if (m_MaxStackText) m_MaxStackText.text = itemEntry.IsUnique
                ? "Unique"
                : $"/ {itemEntry.MaxStack}";

            if (m_UniqueBadge) m_UniqueBadge.SetActive(itemEntry.IsUnique);
        }
    }
}
