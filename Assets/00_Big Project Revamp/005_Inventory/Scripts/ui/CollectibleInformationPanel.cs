using System.Collections.Generic;
using LegionKnight;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    /// <summary>
    /// Panel informasi yang dipakai bersama oleh Hero, Card, dan Item.
    ///
    /// Cara kerja
    /// ──────────
    ///  1. Player memilih collectible di panel manapun (Hero / Card / Item).
    ///  2. Panel tersebut memanggil CollectibleInformationPanel.Bind(entry).
    ///  3. Panel iterasi semua m_DetailSections:
    ///       - Setiap section memanggil IsRelevantFor(entry).
    ///       - Jika relevan → OnBind() + Show().
    ///       - Jika tidak   → Hide().
    ///  4. Tekan tombol "Detail" → panel ini tampil (Show()).
    ///
    /// Setup di Inspector
    /// ──────────────────
    ///  - Tambahkan semua CollectibleDetailSection ke m_DetailSections.
    ///  - Urutan tidak penting — tiap section mengatur dirinya sendiri.
    /// </summary>
    public class CollectibleInformationPanel : PanelView
    {
        [SerializeField]
        private List<CollectibleDetailSection> m_DetailSections = new();

        [SerializeField]
        private Button m_CloseButton;

        // Entry yang sedang ditampilkan
        private ICollectibleEntry m_CurrentEntry;
        public ICollectibleEntry CurrentEntry => m_CurrentEntry;

        // ── Unity ─────────────────────────────────────────────────────
        protected virtual void Awake()
        {
            if (m_CloseButton) m_CloseButton.onClick.AddListener(HideInternal);
        }

        // ── Bind ──────────────────────────────────────────────────────

        /// <summary>
        /// Bind entry ke panel.  Panggil ini sebelum Show().
        /// Panel tidak otomatis Show() — caller yang memutuskan kapan panel tampil.
        /// </summary>
        public void Bind(ICollectibleEntry entry)
        {
            BindInternal(entry);
        }

        protected virtual void BindInternal(ICollectibleEntry entry)
        {
            m_CurrentEntry = entry;
            foreach (CollectibleDetailSection section in m_DetailSections)
                section.Bind(entry);
        }

        /// <summary>Shortcut: bind lalu langsung tampilkan panel.</summary>
        public void BindAndShow(ICollectibleEntry entry)
        {
            BindAndShowInternal(entry);
        }

        protected virtual void BindAndShowInternal(ICollectibleEntry entry)
        {
            BindInternal(entry);
            ShowInternal();
        }

        /// <summary>Bersihkan tampilan dan sembunyikan panel.</summary>
        public void Clear()
        {
            ClearInternal();
        }

        protected virtual void ClearInternal()
        {
            m_CurrentEntry = null;
            foreach (CollectibleDetailSection section in m_DetailSections)
                section.Hide();
            HideInternal();
        }

        // ── Section management (runtime, jika perlu inject section) ───
        public void AddSection(CollectibleDetailSection section)
        {
            AddSectionInternal(section);
        }

        protected virtual void AddSectionInternal(CollectibleDetailSection section)
        {
            if (section == null || m_DetailSections.Contains(section)) return;
            m_DetailSections.Add(section);

            // Jika ada entry aktif, langsung bind section baru
            if (m_CurrentEntry != null) section.Bind(m_CurrentEntry);
        }

        public void RemoveSection(CollectibleDetailSection section)
        {
            RemoveSectionInternal(section);
        }

        protected virtual void RemoveSectionInternal(CollectibleDetailSection section)
        {
            m_DetailSections.Remove(section);
        }
    }
}