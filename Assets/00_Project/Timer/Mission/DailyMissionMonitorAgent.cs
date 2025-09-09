using System.Linq;
using UnityEngine;

namespace LegionKnight
{
    public class DailyMissionMonitorAgent : MonoBehaviour
    {
        public void SetTaskProgressSlide(MissionController controller)
        {
            float powerRate = (float)controller.CurrentTaskPower / (float)controller.MaxTaskPower;
            GameManager.Instance.GetDailyMissionMonitor().SetTaskProgressSlide(powerRate);
        }
        public void Init(MissionController controller)
        {
            GameManager.Instance.GetDailyMissionMonitor().Init(controller);
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
