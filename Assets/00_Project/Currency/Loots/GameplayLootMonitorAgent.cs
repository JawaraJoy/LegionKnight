using UnityEngine;

namespace LegionKnight
{
    public class GameplayLootMonitorAgent : MonoBehaviour
    {
        private GameplayLootMonitor GetGameplayLootMonitor()
        {
            return GameManager.Instance.GetGameplayLootMonitor();
        }
        public void SpawnLoot(LootField loot)
        {
            GetGameplayLootMonitor().SpawnLootView(loot);
        }
        public void ClearAllLootViews()
        {
            GetGameplayLootMonitor().ClearAllLootViews();
        }
    }
}
