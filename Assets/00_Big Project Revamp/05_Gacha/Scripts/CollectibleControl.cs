using UnityEngine;

namespace Rush
{
    // Stub — extend sesuai inventory system yang sudah ada
    public class CollectibleControl : MonoBehaviour
    {
        public void AddCollectible(CollectibleConfig config, int amount)
        {
            if (config == null) return;
            Debug.Log($"[CollectibleControl] +{amount}x {config.name}");
            // Sambungkan ke InventoryManager / CollectionManager di sini

            LootField.CharacterApplier(config);
            LootField.CurrencyApplier(config, amount);
            LootField.CardApplier(config, amount);
            LootField.EnergyApplier(config, amount);
        }
        public static void AddCollectibleStatic(string source, CollectibleConfig config, int amount)
        {
            if (config == null) return;
            Debug.Log($"[CollectibleControl] +{amount}x {config.name}");
            // Sambungkan ke InventoryManager / CollectionManager di sini
            LootField.CharacterApplier(config);
            LootField.CurrencyApplier(config, amount);
            LootField.CardApplier(config, amount);
            LootField.EnergyApplier(config, amount);

            config.OnCollect(source, amount);
        }
    }
}