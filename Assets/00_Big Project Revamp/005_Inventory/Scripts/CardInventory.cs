using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rush
{
    /// <summary>
    /// Manages the full card roster (owned + locked).
    ///
    /// Usage
    /// ─────
    ///   cardInventory.RegisterAll(allCardConfigs, defaultOwnedConfigs);
    ///   cardInventory.Unlock(someCardConfig);
    ///   cardInventory.TrySelectCard(someCardConfig);
    /// </summary>
    public class CardInventory : InventoryBase<CardConfig, CardInventoryEntry>
    {
        // ── Selection cap ─────────────────────────────────────────────
        private int m_MaxSelected;
        public int MaxSelected => m_MaxSelected;

        // ── Events ────────────────────────────────────────────────────
        public event Action<CardInventoryEntry> OnCardUnlocked;
        public event Action<CardInventoryEntry> OnCardSelected;
        public event Action<CardInventoryEntry> OnCardDeselected;

        // ── Constructor ───────────────────────────────────────────────
        /// <param name="maxSelected">Deck size cap. 0 = unlimited.</param>
        public CardInventory(int maxSelected = 0)
        {
            m_MaxSelected = maxSelected;
        }

        // ── Registration ──────────────────────────────────────────────
        public void RegisterAll(IEnumerable<CardConfig> allConfigs,
                                IEnumerable<CardConfig> defaultOwned = null)
        {
            RegisterAllInternal(allConfigs, defaultOwned);
        }

        protected virtual void RegisterAllInternal(IEnumerable<CardConfig> allConfigs,
                                                   IEnumerable<CardConfig> defaultOwned = null)
        {
            HashSet<CardConfig> defaultSet = defaultOwned != null
                ? new HashSet<CardConfig>(defaultOwned)
                : new HashSet<CardConfig>();

            foreach (CardConfig config in allConfigs)
            {
                bool owned = defaultSet.Contains(config);
                TryAdd(new CardInventoryEntry(config, owned));
            }
        }

        // ── Unlock ────────────────────────────────────────────────────
        public bool Unlock(CardConfig config)
        {
            return UnlockInternal(config);
        }

        protected virtual bool UnlockInternal(CardConfig config)
        {
            CardInventoryEntry entry = Get(config);
            if (entry == null)
            {
                Debug.LogWarning($"[CardInventory] Card not registered: {config?.BaseInfo?.Name}");
                return false;
            }
            if (entry.IsOwned) return false;

            entry.Unlock();
            OnCardUnlocked?.Invoke(entry);
            return true;
        }

        // ── Selection ─────────────────────────────────────────────────
        public IReadOnlyList<CardInventoryEntry> SelectedCards
            => SelectedCardsInternal;

        protected IReadOnlyList<CardInventoryEntry> SelectedCardsInternal
            => m_Entries.Where(e => e.IsSelected).ToList();

        public bool TrySelectCard(CardConfig config)
        {
            CardInventoryEntry entry = Get(config);
            if (entry == null) return false;
            if (entry.IsSelected) return true;

            if (m_MaxSelected > 0 && SelectedCardsInternal.Count >= m_MaxSelected)
            {
                Debug.LogWarning($"[CardInventory] Deck cap ({m_MaxSelected}) reached.");
                return false;
            }

            bool ok = entry.TrySelect();
            if (ok) OnCardSelected?.Invoke(entry);
            return ok;
        }

        public void DeselectCard(CardConfig config)
        {
            DeselectCardInternal(config);
        }

        protected virtual void DeselectCardInternal(CardConfig config)
        {
            CardInventoryEntry entry = Get(config);
            if (entry == null || !entry.IsSelected) return;
            entry.Deselect();
            OnCardDeselected?.Invoke(entry);
        }

        public void DeselectAll()
        {
            DeselectAllInternal();
        }

        protected virtual void DeselectAllInternal()
        {
            foreach (CardInventoryEntry e in m_Entries.Where(e => e.IsSelected))
            {
                e.Deselect();
                OnCardDeselected?.Invoke(e);
            }
        }

        // ── Queries ───────────────────────────────────────────────────
        public IReadOnlyList<CardInventoryEntry> OwnedCards
            => m_Entries.Where(e => e.IsOwned).ToList();

        public IReadOnlyList<CardInventoryEntry> LockedCards
            => m_Entries.Where(e => !e.IsOwned).ToList();

        // ── InventoryBase override ────────────────────────────────────
        protected override bool ValidateAdd(CardInventoryEntry entry, out string reason)
        {
            // Cards are always unique per config
            if (Contains(entry.Config))
            {
                reason = "card config already registered in the inventory.";
                return false;
            }
            reason = null;
            return true;
        }
    }
}