using UnityEngine;

namespace LegionKnight
{
    public class GameplayLootMonitorAgent : MonoBehaviour
    {
        private GameplayLootMonitor GetGameplayLootMonitor()
        {
            return CanvasManager.Instance.GetGameplayLootMonitor();
        }
        public void AddLootView(LootField loot)
        {
            GetGameplayLootMonitor().AddLootView(loot);
        }
        public void RemoveLootView(LootField loot)
        {
            GetGameplayLootMonitor().RemoveLootView(loot);
        }
        public void ClearAllLootViews()
        {
            GetGameplayLootMonitor().ClearAllLootViews();
        }
    }
}
