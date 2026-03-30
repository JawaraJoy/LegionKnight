using UnityEngine;

namespace Rush
{
    /// <summary>
    /// Interface yang menjembatani UI dengan berbagai tipe InventoryEntry.
    /// CollectibleDetailSection hanya perlu tahu ICollectibleEntry,
    /// bukan tipe konkret HeroInventoryEntry / CardInventoryEntry / ItemInventoryEntry.
    ///
    /// Setiap InventoryEntry yang ingin ditampilkan di info panel
    /// harus implement interface ini.
    /// </summary>
    public interface ICollectibleEntry
    {
        // ── Data dasar (dari BaseInfo + CollectibleField) ──────────────
        string      Id          { get; }
        string      Name        { get; }
        string      Description { get; }
        Sprite      Icon        { get; }
        Sprite      SplashImage { get; }
        RarityConfig RarityConfig { get; }

        // ── Kepemilikan ────────────────────────────────────────────────
        /// <summary>True jika player sudah memiliki collectible ini.</summary>
        bool IsOwned { get; }
    }
}
