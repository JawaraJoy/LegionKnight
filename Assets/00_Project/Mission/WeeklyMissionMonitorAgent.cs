using System.Linq;
using UnityEngine;

namespace LegionKnight
{
    public class WeeklyMissionMonitorAgent : MonoBehaviour
    {
        public void Init(MissionController controller)
        {
            WeeklyMissionMonitor weeklyMissionMonitor = GameManager.Instance.GetWeeklyMissionMonitor();

            if(weeklyMissionMonitor)
                weeklyMissionMonitor.Init(controller);
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
