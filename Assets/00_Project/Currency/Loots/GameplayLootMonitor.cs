using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class GameplayLootMonitor : LootMonitor
    {
        
    }

    public partial class CanvasManager
    {
        public GameplayLootMonitor GetGameplayLootMonitor()
        {
            GameplayPanel gameplayPanel = GetPanelInternal<GameplayPanel>();
            return gameplayPanel.GetBinding<GameplayLootMonitor>();
        }
    }
}
