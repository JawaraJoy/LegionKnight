using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    /// <summary>
    /// Abstract base untuk tiap "kartu" yang tampil di grid inventory.
    /// TEntry = tipe InventoryEntry yang card ini representasikan.
    ///
    /// Minimal menampilkan: icon, nama, locked overlay.
    /// Subclass dapat menambah komponen tambahan (stars, quantity, dll).
    /// </summary>
    public abstract class CollectibleWidget<TEntry> : MonoBehaviour
        where TEntry : class, ICollectibleEntry
    {
        [Header("Base UI")]
        [SerializeField] private Image m_IconImage;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private GameObject m_LockedOverlay;
        [SerializeField] private GameObject m_SelectedHighlight;
        [SerializeField] private Button m_Button;

        // ── State ─────────────────────────────────────────────────────
        public TEntry Entry { get; private set; }

        private Action<TEntry> m_OnClick;

        // ── Bind ──────────────────────────────────────────────────────
        public void Bind(TEntry entry, Action<TEntry> onClick)
        {
            BindInternal(entry, onClick);
        }

        protected virtual void BindInternal(TEntry entry, Action<TEntry> onClick)
        {
            Entry = entry;
            m_OnClick = onClick;

            if (m_IconImage) { m_IconImage.sprite = entry.Icon; m_IconImage.enabled = entry.Icon != null; }
            if (m_NameText) m_NameText.text = entry.Name ?? string.Empty;
            if (m_LockedOverlay) m_LockedOverlay.SetActive(!entry.IsOwned);

            if (m_Button)
            {
                m_Button.onClick.RemoveAllListeners();
                m_Button.onClick.AddListener(() => m_OnClick?.Invoke(Entry));
            }

            OnBind(entry);
        }

        /// <summary>Override untuk binding komponen tambahan di subclass.</summary>
        protected virtual void OnBind(TEntry entry) { }

        // ── Selection highlight ───────────────────────────────────────
        public void SetSelected(bool selected)
        {
            SetSelectedInternal(selected);
        }

        protected virtual void SetSelectedInternal(bool selected)
        {
            if (m_SelectedHighlight) m_SelectedHighlight.SetActive(selected);
            OnSelectionChanged(selected);
        }

        /// <summary>Override jika perlu reaksi visual tambahan saat selected/deselected.</summary>
        protected virtual void OnSelectionChanged(bool selected) { }
    }
}