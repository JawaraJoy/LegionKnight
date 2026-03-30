using System.Collections.Generic;
using LegionKnight;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    /// <summary>
    /// Abstract base untuk semua inventory panel (Hero, Card, Item).
    ///
    /// Bertanggung jawab atas:
    ///  - Spawn dan refresh grid collectible card
    ///  - Highlight collectible yang sedang dipilih
    ///  - Tombol "Detail" → CollectibleInformationPanel.BindAndShow()
    ///
    /// TEntry  = tipe InventoryEntry (HeroInventoryEntry, CardInventoryEntry, ItemInventoryEntry)
    /// TWidget = tipe MonoBehaviour UI card di grid (CollectibleWidget turunan)
    /// </summary>
    public abstract class CollectiblePanelView<TEntry, TWidget> : PanelView
        where TEntry : class, ICollectibleEntry
        where TWidget : CollectibleWidget<TEntry>
    {
        [Header("Grid")]
        [SerializeField] private Transform m_GridContainer;
        [SerializeField] private TWidget m_WidgetPrefab;

        [Header("Detail button")]
        [SerializeField] private Button m_DetailButton;

        [Header("Info panel reference")]
        [SerializeField] private CollectibleInformationPanel m_InformationPanel;

        // ── State ─────────────────────────────────────────────────────
        private readonly List<TWidget> m_ActiveWidgets = new();
        private TEntry m_SelectedEntry;

        // ── Unity ─────────────────────────────────────────────────────
        protected virtual void Awake()
        {
            if (m_DetailButton)
                m_DetailButton.onClick.AddListener(OnDetailButtonClicked);
        }

        // ── Refresh ───────────────────────────────────────────────────

        /// <summary>
        /// Rebuild seluruh grid dari daftar entry.
        /// Panggil ini setelah inventory berubah (unlock, add, dll).
        /// </summary>
        public void Refresh(IReadOnlyList<TEntry> entries)
        {
            RefreshInternal(entries);
        }

        protected virtual void RefreshInternal(IReadOnlyList<TEntry> entries)
        {
            ClearGridInternal();

            foreach (TEntry entry in entries)
            {
                if (!ShouldShowEntry(entry)) continue;

                TWidget widget = Instantiate(m_WidgetPrefab, m_GridContainer);
                widget.Bind(entry, OnWidgetClicked);
                m_ActiveWidgets.Add(widget);
            }

            if (m_SelectedEntry != null)
                RefreshSelectionInternal();
        }

        private void ClearGridInternal()
        {
            foreach (TWidget w in m_ActiveWidgets)
                if (w) Destroy(w.gameObject);
            m_ActiveWidgets.Clear();
        }

        // ── Visibility filter ─────────────────────────────────────────

        /// <summary>
        /// Override untuk mengontrol entry mana yang muncul di grid.
        /// Default: tampilkan semua.
        /// ItemPanelView override ini untuk hanya menampilkan entry dengan Quantity > 0.
        /// </summary>
        protected virtual bool ShouldShowEntry(TEntry entry) => true;

        // ── Selection ─────────────────────────────────────────────────
        private void OnWidgetClicked(TEntry entry)
        {
            m_SelectedEntry = entry;
            RefreshSelectionInternal();
            OnEntrySelected(entry);

            if (m_InformationPanel) m_InformationPanel.Bind(entry);
        }

        private void RefreshSelectionInternal()
        {
            foreach (TWidget w in m_ActiveWidgets)
                w.SetSelected(w.Entry == m_SelectedEntry);
        }

        /// <summary>
        /// Dipanggil setelah player mengklik sebuah widget.
        /// Override untuk logika tambahan (misalnya enable/disable tombol tertentu).
        /// </summary>
        protected virtual void OnEntrySelected(TEntry entry) { }

        // ── Detail button ─────────────────────────────────────────────
        private void OnDetailButtonClicked()
        {
            if (m_SelectedEntry == null || m_InformationPanel == null) return;
            m_InformationPanel.BindAndShow(m_SelectedEntry);
        }
    }
}