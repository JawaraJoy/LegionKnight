using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rush
{
    /// <summary>
    /// Generic base inventory.
    /// TConfig  = CollectibleConfig subclass (HeroUnitConfig, CardConfig, ItemConfig …)
    /// TEntry   = InventoryEntry subclass for that config type
    /// </summary>
    public abstract class InventoryBase<TConfig, TEntry>
        where TConfig : CollectibleConfig
        where TEntry : InventoryEntry<TConfig>
    {
        // ── Storage ───────────────────────────────────────────────────
        protected readonly List<TEntry> m_Entries = new();

        // ── Events ────────────────────────────────────────────────────
        public event Action<TEntry> OnEntryAdded;
        public event Action<TEntry> OnEntryRemoved;

        // ── Read ──────────────────────────────────────────────────────
        public IReadOnlyList<TEntry> All => m_Entries;
        public int Count => m_Entries.Count;

        /// <summary>Returns the entry whose config matches, or null.</summary>
        public TEntry Get(TConfig config)
            => m_Entries.FirstOrDefault(e => e.Config == config);

        /// <summary>Returns the entry by its instance id, or null.</summary>
        public TEntry Get(string instanceId)
            => m_Entries.FirstOrDefault(e => e.InstanceId == instanceId);

        public bool Contains(TConfig config) => Get(config) != null;

        // ── Write ─────────────────────────────────────────────────────
        public bool TryAdd(TEntry entry)
        {
            return TryAddInternal(entry);
        }

        protected virtual bool TryAddInternal(TEntry entry)
        {
            if (entry == null)
            {
                Debug.LogWarning($"[Inventory] TryAdd received a null entry.");
                return false;
            }

            if (!ValidateAdd(entry, out string reason))
            {
                Debug.LogWarning($"[Inventory] Cannot add '{entry.Config?.BaseInfo?.Name}': {reason}");
                return false;
            }

            m_Entries.Add(entry);
            entry.OnAddedToInventory();
            OnEntryAdded?.Invoke(entry);
            return true;
        }

        /// <summary>
        /// Removes the entry that matches the given config.
        /// Returns false if not found.
        /// </summary>
        public bool TryRemove(TConfig config)
        {
            TEntry entry = Get(config);
            if (entry == null) return false;
            return TryRemoveInternal(entry);
        }

        public bool TryRemove(TEntry entry)
        {
            return TryRemoveInternal(entry);
        }

        protected virtual bool TryRemoveInternal(TEntry entry)
        {
            if (!m_Entries.Remove(entry)) return false;
            entry.OnRemovedFromInventory();
            OnEntryRemoved?.Invoke(entry);
            return true;
        }

        // ── Override hook ─────────────────────────────────────────────

        /// <summary>
        /// Subclasses implement domain-specific checks here.
        /// Return false + set reason to block the add.
        /// </summary>
        protected abstract bool ValidateAdd(TEntry entry, out string reason);
    }
}