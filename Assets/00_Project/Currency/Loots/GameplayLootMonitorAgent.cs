using UnityEngine;

namespace LegionKnight
{
    public class GameplayLootMonitorAgent : MonoBehaviour
    {
        private GameplayLootMonitor GetGameplayLootMonitor()
        {
            return GameManager.Instance.GetGameplayLootMonitor();
        }
        public void AddLootView(LootField loot)
        {
            GetGameplayLootMonitor().AddLootView(loot);
        }
        public void ClearAllLootViews()
        {
            GetGameplayLootMonitor().ClearAllLootViews();
        }
    }
}
