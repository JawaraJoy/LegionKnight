using UnityEngine;

namespace Rush
{
    // Compatibility entrypoint for existing project callers and serialized integrations.
    public class CollectibleControl : MonoBehaviour
    {
        public static void AddCollectibleStatic(string source, CollectibleConfig config, int amount)
        {
            if (config == null || amount <= 0) throw new System.ArgumentException("Invalid collectible reward.");
            Debug.Log($"[CollectibleControl] +{amount}x {config.name}");
            // Sambungkan ke InventoryManager / CollectionManager di sini
            LootField.CharacterApplier(config);
            LootField.CurrencyApplier(config, amount);
            LootField.CardApplier(config, amount);
            LootField.EnergyApplier(config, amount);

            // Analytics must never turn a successful inventory grant into a failed transaction.
            if (!Application.isPlaying) return;
            try { config.OnCollect(source, amount); }
            catch (System.Exception ex) { Debug.LogWarning($"[CollectibleControl] Analytics failed: {ex.Message}"); }
        }
    }
}