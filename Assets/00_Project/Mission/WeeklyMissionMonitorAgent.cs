using System.Linq;
using UnityEngine;

namespace LegionKnight
{
    public class WeeklyMissionMonitorAgent : MonoBehaviour
    {
        public void Init(MissionController controller)
        {
            GameManager.Instance.GetWeeklyMissionMonitor().Init(controller);
        }
        public void ShowLoot(LootField[] loots)
        {
            LootMonitor lootMonitor = GameManager.Instance.GetLootMonitor();
            lootMonitor.ClearAllLootViews();
            lootMonitor.Show();
            lootMonitor.AddLootsView(loots.ToList());
        }
    }
}
