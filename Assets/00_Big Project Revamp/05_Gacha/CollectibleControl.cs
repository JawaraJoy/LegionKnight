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
        }
    }
}