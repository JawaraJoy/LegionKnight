using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class LootStorageManager : LootStorage { }

    public partial class GameManager
    {
        [SerializeField]
        private LootStorageManager m_LootStorageManager;
        public List<LootField> Loots => m_LootStorageManager.Looteds;

        public LootStorageManager GetLootStorageManager()
        {
            return m_LootStorageManager;
        }
        public void TakeLooteds()
        {
            m_LootStorageManager.TakeLooteds();
        }
        public void AddLoots(LootField[] loots)
        {
            m_LootStorageManager.AddLoots(loots);
        }
        public void AddLoot(LootField loot)
        {
            m_LootStorageManager.AddLoot(loot);
        }
        public void ClearLoots()
        {
            m_LootStorageManager.ClearLoots();
        }
    }
}
