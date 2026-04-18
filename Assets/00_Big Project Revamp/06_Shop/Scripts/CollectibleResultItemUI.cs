using UnityEngine;

namespace Rush
{
    // Rename dari GachaResultItemUI → CollectibleResultItemUI
    // Setup kini menerima CollectibleResultEntry (generic)
    // GachaResultItemUI tidak dihapus — cukup jadikan alias via subclass kosong
    // agar prefab lama tidak perlu diubah
    public class CollectibleResultItemUI : GachaCollectableItemUI
    {
        public void Setup(CollectibleResultEntry entry)
        {
            SetupBase(entry.Collectible, entry.Amount);
            OnSetupComplete(entry.Collectible, entry.Amount);
        }
    }
}