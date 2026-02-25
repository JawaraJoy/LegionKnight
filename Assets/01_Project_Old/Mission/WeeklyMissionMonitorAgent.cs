using Rush;
using System.Linq;
using UnityEngine;

namespace LegionKnight
{
    public class WeeklyMissionMonitorAgent : MonoBehaviour
    {
        public void Init(MissionController controller)
        {
            WeeklyMissionMonitor weeklyMissionMonitor = CanvasManager.Instance.GetWeeklyMissionMonitor();

            if(weeklyMissionMonitor)
                weeklyMissionMonitor.Init(controller);
        }
        public void ShowLoot(LootField[] loots)
        {
            LootMonitor lootMonitor = CanvasManager.Instance.GetLootMonitor();
            lootMonitor.ClearAllLootViews();
            lootMonitor.Show();
            lootMonitor.AddLootsView(loots.ToList());
        }
    }
}
