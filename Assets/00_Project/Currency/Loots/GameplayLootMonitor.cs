using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class GameplayLootMonitor : LootMonitor
    {
        
    }

    public partial class GameManager
    {
        public GameplayLootMonitor GetGameplayLootMonitor()
        {
            GameplayPanel gameplayPanel = GetPanelInternal<GameplayPanel>();
            return gameplayPanel.GetBinding<GameplayLootMonitor>();
        }
    }
}
