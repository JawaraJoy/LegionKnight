using System;

namespace Rush
{
    /// <summary>
    /// One card slot in the player's card inventory.
    ///
    /// Ownership model (mirrors Hero)
    /// ────────────────────────────────
    ///  - All registered cards appear in the UI (locked / unlocked).
    ///  - IsOwned   = player has acquired this card.
    ///  - IsSelected = card is in the current preparation / deck selection.
    /// </summary>
    [Serializable]
    public partial class CardInventoryEntry : InventoryEntry<CardConfig>
    {
        // ── Ownership ─────────────────────────────────────────────────
        private bool m_IsOwned;
        public bool IsOwned => m_IsOwned;

        // ── Selection (preparation deck) ──────────────────────────────
        private bool m_IsSelected;
        public bool IsSelected => m_IsSelected;

        // ── Events ────────────────────────────────────────────────────
        public event Action<CardInventoryEntry> OnOwnershipChanged;
        public event Action<CardInventoryEntry> OnSelectionChanged;

        // ── Constructor ───────────────────────────────────────────────
        /// <param name="ownedAtStart">Pass true for cards that should start unlocked.</param>
        public CardInventoryEntry(CardConfig config, bool ownedAtStart = false) : base(config)
        {
            m_IsOwned = ownedAtStart;
        }

        // ── Ownership ─────────────────────────────────────────────────
        public void Unlock()
        {
            UnlockInternal();
        }

        protected virtual void UnlockInternal()
        {
            if (m_IsOwned) return;
            m_IsOwned = true;
            OnOwnershipChanged?.Invoke(this);
        }

        // ── Selection ─────────────────────────────────────────────────
        public bool TrySelect()
        {
            return TrySelectInternal();
        }

        protected virtual bool TrySelectInternal()
        {
            if (!m_IsOwned) return false;
            if (m_IsSelected) return true;
            m_IsSelected = true;
            OnSelectionChanged?.Invoke(this);
            return true;
        }

        public void Deselect()
        {
            DeselectInternal();
        }

        protected virtual void DeselectInternal()
        {
            if (!m_IsSelected) return;
            m_IsSelected = false;
            OnSelectionChanged?.Invoke(this);
        }
    }
}