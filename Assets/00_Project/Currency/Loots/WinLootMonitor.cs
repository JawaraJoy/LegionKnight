using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class WinLootMonitor : LootMonitor
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
            base.ShowInternal();
            SpawnLootsViewInternal(GetLootStorage().Looteds);
        }
    }
    public partial class GameManager
    {
        public WinLootMonitor GetLevelCompleteLootMonitor()
        {
            WinPanel gameplayPanel = GetPanelInternal<WinPanel>();
            return gameplayPanel.GetBinding<WinLootMonitor>();
        }
    }
}
