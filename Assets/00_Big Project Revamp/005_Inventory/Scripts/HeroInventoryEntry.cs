using System;
using UnityEngine;

namespace Rush
{
    /// <summary>
    /// One hero slot in the player's hero inventory.
    ///
    /// Ownership model
    /// ───────────────
    ///  - Heroes always appear in the UI (locked / unlocked).
    ///  - IsOwned   = player has acquired this hero.
    ///  - IsDefault = hero is pre-owned at first launch (from HeroUnitConfig.OwnedAtFirst).
    ///  - IsSelected = hero is currently chosen for the preparation / battle lineup.
    /// </summary>
    [Serializable]
    public partial class HeroInventoryEntry : InventoryEntry<HeroUnitConfig>
    {
        // ── Ownership ─────────────────────────────────────────────────
        private bool m_IsOwned;
        public bool IsOwned => m_IsOwned;

        /// <summary>True when the hero comes pre-unlocked via HeroUnitConfig.OwnedAtFirst.</summary>
        public bool IsDefault => Config.OwnedAtFirst;

        // ── Progression ───────────────────────────────────────────────
        private int m_CurrentStars;
        public int CurrentStars => m_CurrentStars;
        public int MaxStars => Config.MaxStars;

        private int m_CurrentLevel;
        public int CurrentLevel => m_CurrentLevel;

        // ── Selection (preparation / squad lineup) ────────────────────
        private bool m_IsSelected;
        public bool IsSelected => m_IsSelected;

        // ── Events ────────────────────────────────────────────────────
        public event Action<HeroInventoryEntry> OnOwnershipChanged;
        public event Action<HeroInventoryEntry> OnSelectionChanged;
        public event Action<HeroInventoryEntry> OnProgressionChanged;

        // ── Constructor ───────────────────────────────────────────────
        public HeroInventoryEntry(HeroUnitConfig config) : base(config)
        {
            m_IsOwned = config.OwnedAtFirst;
            m_CurrentStars = config.StartingStars;
            m_CurrentLevel = 1;
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
            if (!m_IsOwned)
            {
                Debug.LogWarning($"[HeroInventory] Cannot select locked hero: {Config.BaseInfo?.Name}");
                return false;
            }
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

        // ── Progression ───────────────────────────────────────────────
        public void SetLevel(int level)
        {
            SetLevelInternal(level);
        }

        protected virtual void SetLevelInternal(int level)
        {
            m_CurrentLevel = Mathf.Max(1, level);
            OnProgressionChanged?.Invoke(this);
        }

        public bool TryAddStar()
        {
            return TryAddStarInternal();
        }

        protected virtual bool TryAddStarInternal()
        {
            if (m_CurrentStars >= MaxStars) return false;
            m_CurrentStars++;
            OnProgressionChanged?.Invoke(this);
            return true;
        }

        public override void OnAddedToInventory()
        {
            if (Config.UseAsDefault && m_IsOwned)
                m_IsSelected = true;
        }
    }
}