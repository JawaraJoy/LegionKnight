using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    /// <summary>
    /// Section yang menampilkan detail khusus hero:
    /// stars (breakthrough), level, faction, progression formula.
    ///
    /// Hanya tampil ketika entry yang di-bind adalah HeroInventoryEntry.
    /// </summary>
    public class HeroDetailSection : CollectibleDetailSection
    {
        [Header("Stars / Breakthrough")]
        [SerializeField] private Transform          m_StarsContainer;
        [SerializeField] private GameObject         m_StarFilledPrefab;
        [SerializeField] private GameObject         m_StarEmptyPrefab;

        [Header("Level")]
        [SerializeField] private TextMeshProUGUI    m_LevelText;
        [SerializeField] private Slider             m_LevelProgressBar;

        [Header("Faction")]
        [SerializeField] private TextMeshProUGUI    m_FactionText;
        [SerializeField] private Image              m_FactionIcon;

        [Header("Ownership badge")]
        [SerializeField] private GameObject         m_LockedOverlay;
        [SerializeField] private TextMeshProUGUI    m_OwnershipLabel;

        // ── CollectibleDetailSection ──────────────────────────────────
        public override bool IsRelevantFor(ICollectibleEntry entry)
        {
            // Hanya tampil untuk hero entry
            return entry is HeroInventoryEntry;
        }

        protected override void OnBind(ICollectibleEntry entry)
        {
            if (entry is not HeroInventoryEntry heroEntry) return;

            BindStars(heroEntry);
            BindLevel(heroEntry);
            BindFaction(heroEntry);
            BindOwnership(heroEntry);
        }

        // ── Stars ─────────────────────────────────────────────────────
        private void BindStars(HeroInventoryEntry entry)
        {
            if (!m_StarsContainer) return;

            // Bersihkan bintang lama
            foreach (Transform child in m_StarsContainer)
                Destroy(child.gameObject);

            int current = entry.CurrentStars;
            int max     = entry.MaxStars;

            for (int i = 0; i < max; i++)
            {
                GameObject prefab = i < current ? m_StarFilledPrefab : m_StarEmptyPrefab;
                if (prefab) Instantiate(prefab, m_StarsContainer);
            }
        }

        // ── Level ─────────────────────────────────────────────────────
        private void BindLevel(HeroInventoryEntry entry)
        {
            if (m_LevelText) m_LevelText.text = $"Lv. {entry.CurrentLevel}";

            // Progress bar — gunakan LevelFormulaConfig jika tersedia
            if (m_LevelProgressBar && entry.Config.LevelFormulaConfig != null)
            {
                // Formula progress diserahkan ke LevelFormulaConfig — di sini hanya
                // contoh normalisasi sederhana. Ganti sesuai implementasi formula kamu.
                m_LevelProgressBar.value = entry.CurrentLevel / 100f;
            }
        }

        // ── Faction ───────────────────────────────────────────────────
        private void BindFaction(HeroInventoryEntry entry)
        {
            FactionConfig faction = entry.Config.Faction;
            if (faction == null)
            {
                if (m_FactionText) m_FactionText.text     = string.Empty;
                if (m_FactionIcon) m_FactionIcon.enabled  = false;
                return;
            }

            if (m_FactionText) m_FactionText.text    = faction.BaseInfo?.Name ?? string.Empty;
            if (m_FactionIcon) m_FactionIcon.enabled = false; // isi icon faction jika ada referensinya
        }

        // ── Ownership ─────────────────────────────────────────────────
        private void BindOwnership(HeroInventoryEntry entry)
        {
            if (m_LockedOverlay)   m_LockedOverlay.SetActive(!entry.IsOwned);
            if (m_OwnershipLabel)  m_OwnershipLabel.text = entry.IsOwned ? "Owned" : "Locked";
        }
    }
}
