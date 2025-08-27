using UnityEngine;

namespace LegionKnight
{
    public class PreviewLootMonitor : LootMonitor
    {
        private LootStorage m_LootStorage;
        private LootStorage GetLootStorage()
        {
            if (m_LootStorage == null)
            {
                m_LootStorage = GameManager.Instance.GetLootStorageManager();
            }
            return m_LootStorage;
        }
        protected override void ShowInternal()
        {
            ClearAllLootViews();
            base.ShowInternal();
            SpawnLootsViewInternal(GetLootStorage().Looteds);
        }
    }
}
