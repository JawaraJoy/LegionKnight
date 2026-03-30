using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rush
{
    /// <summary>
    /// Manages the full roster of heroes (owned + locked).
    ///
    /// Usage
    /// ─────
    ///   // Populate once (e.g. from all HeroUnitConfig assets)
    ///   heroInventory.RegisterAll(allHeroConfigs);
    ///
    ///   // Unlock when player earns a hero
    ///   heroInventory.Unlock(someHeroConfig);
    ///
    ///   // Selection for preparation screen
    ///   heroInventory.TrySelectHero(someHeroConfig);
    ///   heroInventory.DeselectHero(someHeroConfig);
    /// </summary>
    public class HeroInventory : InventoryBase<HeroUnitConfig, HeroInventoryEntry>
    {
        // ── Selection cap ─────────────────────────────────────────────
        private int m_MaxSelected;
        public int MaxSelected => m_MaxSelected;

        // ── Events ────────────────────────────────────────────────────
        public event Action<HeroInventoryEntry> OnHeroUnlocked;
        public event Action<HeroInventoryEntry> OnHeroSelected;
        public event Action<HeroInventoryEntry> OnHeroDeselected;

        // ── Constructor ───────────────────────────────────────────────
        /// <param name="maxSelected">How many heroes can be in the preparation lineup at once. 0 = unlimited.</param>
        public HeroInventory(int maxSelected = 0)
        {
            m_MaxSelected = maxSelected;
        }

        // ── Registration ──────────────────────────────────────────────
        public void RegisterAll(IEnumerable<HeroUnitConfig> allConfigs)
        {
            RegisterAllInternal(allConfigs);
        }

        protected virtual void RegisterAllInternal(IEnumerable<HeroUnitConfig> allConfigs)
        {
            foreach (HeroUnitConfig config in allConfigs)
                TryAdd(new HeroInventoryEntry(config));
        }

        // ── Unlock ────────────────────────────────────────────────────
        public bool Unlock(HeroUnitConfig config)
        {
            return UnlockInternal(config);
        }

        protected virtual bool UnlockInternal(HeroUnitConfig config)
        {
            HeroInventoryEntry entry = Get(config);
            if (entry == null)
            {
                Debug.LogWarning($"[HeroInventory] Hero not registered: {config?.BaseInfo?.Name}");
                return false;
            }
            if (entry.IsOwned) return false;

            entry.Unlock();
            OnHeroUnlocked?.Invoke(entry);
            return true;
        }

        // ── Selection ─────────────────────────────────────────────────
        public IReadOnlyList<HeroInventoryEntry> SelectedHeroes
            => SelectedHeroesInternal;

        protected IReadOnlyList<HeroInventoryEntry> SelectedHeroesInternal
            => m_Entries.Where(e => e.IsSelected).ToList();

        public bool TrySelectHero(HeroUnitConfig config)
        {
            HeroInventoryEntry entry = Get(config);
            if (entry == null) return false;
            if (entry.IsSelected) return true;

            if (m_MaxSelected > 0 && SelectedHeroesInternal.Count >= m_MaxSelected)
            {
                Debug.LogWarning($"[HeroInventory] Selection cap ({m_MaxSelected}) reached.");
                return false;
            }

            bool ok = entry.TrySelect();
            if (ok) OnHeroSelected?.Invoke(entry);
            return ok;
        }

        public void DeselectHero(HeroUnitConfig config)
        {
            DeselectHeroInternal(config);
        }

        protected virtual void DeselectHeroInternal(HeroUnitConfig config)
        {
            HeroInventoryEntry entry = Get(config);
            if (entry == null || !entry.IsSelected) return;
            entry.Deselect();
            OnHeroDeselected?.Invoke(entry);
        }

        public void DeselectAll()
        {
            DeselectAllInternal();
        }

        protected virtual void DeselectAllInternal()
        {
            foreach (HeroInventoryEntry e in m_Entries.Where(e => e.IsSelected))
            {
                e.Deselect();
                OnHeroDeselected?.Invoke(e);
            }
        }

        // ── Queries ───────────────────────────────────────────────────
        public IReadOnlyList<HeroInventoryEntry> OwnedHeroes
            => m_Entries.Where(e => e.IsOwned).ToList();

        public IReadOnlyList<HeroInventoryEntry> LockedHeroes
            => m_Entries.Where(e => !e.IsOwned).ToList();

        // ── InventoryBase override ────────────────────────────────────
        protected override bool ValidateAdd(HeroInventoryEntry entry, out string reason)
        {
            // Heroes are always unique per config
            if (Contains(entry.Config))
            {
                reason = "hero config already registered in the inventory.";
                return false;
            }
            reason = null;
            return true;
        }
    }
}