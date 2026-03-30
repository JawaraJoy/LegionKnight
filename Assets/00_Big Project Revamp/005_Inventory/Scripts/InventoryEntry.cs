using System;

namespace Rush
{
    /// <summary>
    /// Base class for a single entry inside any inventory.
    /// T must be a CollectibleConfig (or subclass).
    /// </summary>
    [Serializable]
    public abstract class InventoryEntry<T> where T : CollectibleConfig
    {
        // ── Config reference ──────────────────────────────────────────
        private T m_Config;
        public T Config => m_Config;

        // ── Runtime identity ──────────────────────────────────────────
        /// <summary>Unique runtime id for this entry instance.</summary>
        public string InstanceId { get; private set; }

        /// <summary>UTC timestamp when this entry was added to the inventory.</summary>
        public DateTime AcquiredAt { get; private set; }

        protected InventoryEntry(T config)
        {
            m_Config      = config;
            InstanceId    = Guid.NewGuid().ToString();
            AcquiredAt    = DateTime.UtcNow;
        }

        /// <summary>
        /// Called by the inventory after the entry is constructed and validated.
        /// Override to run config-specific post-init logic.
        /// </summary>
        public virtual void OnAddedToInventory() { }

        /// <summary>
        /// Called by the inventory just before the entry is removed.
        /// </summary>
        public virtual void OnRemovedFromInventory() { }
    }
}
