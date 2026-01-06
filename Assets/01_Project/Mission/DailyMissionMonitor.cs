using UnityEngine;

namespace LegionKnight
{
    public class DailyMissionMonitor : MissionMonitor
    {
        protected override MissionController GetControllerInternal()
        {
            if (m_Controller == null)
            {
                m_Controller = Player.Instance.DailyMissionManager;
            }

            /*
            Debug.Log("xxxxxxxxxxxxxxxx");
            Debug.Log(m_Controller.Task.Length);

            foreach (var item in m_Controller.Task)
            {
                Debug.Log(item.Definition.Id);
            }
            */

            return m_Controller;
        }
    }

    public partial class CanvasManager
    {
        private MissionPanel GetMissionPanel()
        {
            return GetPanelInternal<MissionPanel>();
        }
        public LootMonitor GetLootMonitor()
        {
            return GetMissionPanel().GetBinding<LootMonitor>();
        }
        public DailyMissionMonitor GetDailyMissionMonitor()
        {
            return GetMissionPanel().GetBinding<DailyMissionMonitor>();
        }
    }
}
