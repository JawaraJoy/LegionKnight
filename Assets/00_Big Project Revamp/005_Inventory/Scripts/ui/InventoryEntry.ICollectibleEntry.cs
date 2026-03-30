// ─────────────────────────────────────────────────────────────────────────────
// File ini berisi partial class extension untuk tiap InventoryEntry
// agar mereka implement ICollectibleEntry.
// Letakkan di folder yang sama dengan masing-masing Entry, atau dalam 1 file ini.
// ─────────────────────────────────────────────────────────────────────────────

using UnityEngine;

namespace Rush
{
    // ── Hero ─────────────────────────────────────────────────────────────────
    public partial class HeroInventoryEntry : ICollectibleEntry
    {
        public string       Id           => Config.BaseInfo?.Id;
        public string       Name         => Config.BaseInfo?.Name;
        public string       Description  => Config.BaseInfo?.Description;
        public Sprite       Icon         => Config.CollectibleField?.Icon;
        public Sprite       SplashImage  => Config.CollectibleField?.SplashImage;
        public RarityConfig RarityConfig => Config.CollectibleField?.RarityConfig;
        // IsOwned sudah ada di HeroInventoryEntry
    }

    // ── Card ─────────────────────────────────────────────────────────────────
    public partial class CardInventoryEntry : ICollectibleEntry
    {
        public string       Id           => Config.BaseInfo?.Id;
        public string       Name         => Config.BaseInfo?.Name;
        public string       Description  => Config.BaseInfo?.Description;
        public Sprite       Icon         => Config.CollectibleField?.Icon;
        public Sprite       SplashImage  => Config.CollectibleField?.SplashImage;
        public RarityConfig RarityConfig => Config.CollectibleField?.RarityConfig;
        // IsOwned sudah ada di CardInventoryEntry
    }

    // ── Item ─────────────────────────────────────────────────────────────────
    public partial class ItemInventoryEntry : ICollectibleEntry
    {
        public string       Id           => Config.BaseInfo?.Id;
        public string       Name         => Config.BaseInfo?.Name;
        public string       Description  => Config.BaseInfo?.Description;
        public Sprite       Icon         => Config.CollectibleField?.Icon;
        public Sprite       SplashImage  => Config.CollectibleField?.SplashImage;
        public RarityConfig RarityConfig => Config.CollectibleField?.RarityConfig;

        // Item tidak punya konsep "owned/locked" — selalu true jika ada di inventory
        public bool IsOwned => Quantity > 0;
    }
}
