using System;
using UnityEngine;

namespace Rush
{
    /// <summary>
    /// One item slot in the player's item inventory.
    ///
    /// Stack behaviour
    /// ───────────────
    ///  - If CollectibleField.IsUnique == true  → max quantity is 1 (unique item).
    ///  - If CollectibleField.IsUnique == false → stackable, capped at MaxStack.
    ///  - UI rule: only visible when Quantity > 0.
    /// </summary>
    [Serializable]
    public partial class ItemInventoryEntry : InventoryEntry<ItemConfig>
    {
        // ── Stack ─────────────────────────────────────────────────────
        private int m_Quantity;
        public int Quantity => m_Quantity;

        private readonly int m_MaxStack;
        public int MaxStack => m_MaxStack;

        public bool IsUnique => Config.CollectibleField.IsUnique;

        /// <summary>UI rule: item slot should be visible when this is true.</summary>
        public bool IsVisible => m_Quantity > 0;

        public bool IsFull => m_Quantity >= m_MaxStack;

        // ── Events ────────────────────────────────────────────────────
        public event Action<ItemInventoryEntry> OnQuantityChanged;

        // ── Constructor ───────────────────────────────────────────────
        /// <param name="maxStack">
        /// Ignored when IsUnique == true (capped to 1 automatically).
        /// Pass 0 or negative for unlimited stacking.
        /// </param>
        public ItemInventoryEntry(ItemConfig config, int initialQuantity = 0, int maxStack = 99)
            : base(config)
        {
            // Unique items are always capped at 1
            m_MaxStack = IsUnique ? 1 : (maxStack > 0 ? maxStack : int.MaxValue);
            m_Quantity = Mathf.Clamp(initialQuantity, 0, m_MaxStack);
        }

        // ── Quantity mutations ────────────────────────────────────────
        public int Add(int amount = 1)
        {
            return AddInternal(amount);
        }

        protected virtual int AddInternal(int amount = 1)
        {
            if (amount <= 0) return 0;
            int before = m_Quantity;
            m_Quantity = Mathf.Min(m_Quantity + amount, m_MaxStack);
            int added = m_Quantity - before;
            if (added > 0) OnQuantityChanged?.Invoke(this);
            return added;
        }

        public int Remove(int amount = 1)
        {
            return RemoveInternal(amount);
        }

        protected virtual int RemoveInternal(int amount = 1)
        {
            if (amount <= 0) return 0;
            int before = m_Quantity;
            m_Quantity = Mathf.Max(m_Quantity - amount, 0);
            int removed = before - m_Quantity;
            if (removed > 0) OnQuantityChanged?.Invoke(this);
            return removed;
        }

        public void SetQuantity(int value)
        {
            SetQuantityInternal(value);
        }

        protected virtual void SetQuantityInternal(int value)
        {
            int clamped = Mathf.Clamp(value, 0, m_MaxStack);
            if (clamped == m_Quantity) return;
            m_Quantity = clamped;
            OnQuantityChanged?.Invoke(this);
        }
    }
}