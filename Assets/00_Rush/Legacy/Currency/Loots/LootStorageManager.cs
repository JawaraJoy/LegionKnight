
using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class LootStorageManager : LootStorage { }

    public partial class GameManager // this is singleton
    {
        [SerializeField]
        private LootStorageManager m_LootStorageManager;
        public LootStorageManager LootStorageManager => m_LootStorageManager;
    }
}
