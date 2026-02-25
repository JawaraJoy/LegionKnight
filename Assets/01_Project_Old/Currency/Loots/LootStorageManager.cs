
using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class LootStorageManager : LootStorage { }

    public partial class GameManager
    {
        [SerializeField]
        private LootStorageManager m_LootStorageManager;
        public LootStorageManager LootStorageManager => m_LootStorageManager;
    }
}
