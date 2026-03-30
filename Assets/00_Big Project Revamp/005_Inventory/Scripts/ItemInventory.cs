using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rush
{
    /// <summary>
    /// Manages the player's item stack inventory.
    ///
    /// Key behaviours
    /// ──────────────
    ///  - Items are created on first pickup (lazy registration).
    ///  - Unique items (IsUnique == true) are capped at quantity 1.
    ///  - Stackable items respect their per-entry MaxStack.
    ///  - UI should only show entries where IsVisible == true (Quantity > 0).
    ///
    /// Usage
    /// ─────
    ///   itemInventory.Add(someItemConfig, 3);
    ///   itemInventory.Remove(someItemConfig, 1);
    ///   itemInventory.GetVisible();  // for UI display
    /// </summary>
    public class ItemInventory : InventoryBase<ItemConfig, ItemInventoryEntry>
    {
        // ── Default stack cap (overridable per-entry) ─────────────────
        private readonly int m_DefaultMaxStack;

        // ── Events ────────────────────────────────────────────────────
        public event Action<ItemInventoryEntry> OnQuantityChanged;

        // ── Constructor ───────────────────────────────────────────────
        public ItemInventory(int defaultMaxStack = 99)
        {
            m_DefaultMaxStack = defaultMaxStack;
        }

        // ── Add ───────────────────────────────────────────────────────
        public int Add(ItemConfig config, int amount = 1, int maxStack = -1)
        {
            return AddInternal(config, amount, maxStack);
        }

        protected virtual int AddInternal(ItemConfig config, int amount = 1, int maxStack = -1)
        {
            if (config == null)
            {
                Debug.LogWarning("[ItemInventory] Add called with null config.");
                return 0;
            }

            ItemInventoryEntry entry = GetOrCreateInternal(config, maxStack);
            int added = entry.Add(amount);
            if (added > 0) OnQuantityChanged?.Invoke(entry);
            return added;
        }

        // ── Remove ────────────────────────────────────────────────────
        public int Remove(ItemConfig config, int amount = 1)
        {
            return RemoveInternal(config, amount);
        }

        protected virtual int RemoveInternal(ItemConfig config, int amount = 1)
        {
            ItemInventoryEntry entry = Get(config);
            if (entry == null) return 0;

            int removed = entry.Remove(amount);
            if (removed > 0) OnQuantityChanged?.Invoke(entry);
            return removed;
        }

        // ── Queries ───────────────────────────────────────────────────
        public int GetQuantity(ItemConfig config)
            => GetQuantityInternal(config);

        protected virtual int GetQuantityInternal(ItemConfig config)
            => Get(config)?.Quantity ?? 0;

        public IReadOnlyList<ItemInventoryEntry> GetVisible()
            => GetVisibleInternal();

        protected virtual IReadOnlyList<ItemInventoryEntry> GetVisibleInternal()
            => m_Entries.Where(e => e.IsVisible).ToList();

        public bool HasItem(ItemConfig config, int requiredAmount = 1)
            => HasItemInternal(config, requiredAmount);

        protected virtual bool HasItemInternal(ItemConfig config, int requiredAmount = 1)
            => GetQuantityInternal(config) >= requiredAmount;

        // ── InventoryBase override ────────────────────────────────────
        protected override bool ValidateAdd(ItemInventoryEntry entry, out string reason)
        {
            // Unique check: unique items may only have one entry
            if (entry.IsUnique && Contains(entry.Config))
            {
                reason = "unique item config is already registered.";
                return false;
            }
            reason = null;
            return true;
        }

        // ── Helpers ───────────────────────────────────────────────────
        private ItemInventoryEntry GetOrCreateInternal(ItemConfig config, int maxStack)
        {
            ItemInventoryEntry existing = Get(config);
            if (existing != null) return existing;

            int cap = maxStack > 0 ? maxStack : m_DefaultMaxStack;
            var entry = new ItemInventoryEntry(config, 0, cap);

            entry.OnQuantityChanged += e => OnQuantityChanged?.Invoke(e);

            TryAdd(entry);
            return Get(config);
        }
    }
}