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
            return m_Controller;
        }
    }

    public partial class GameManager
    {
        private MissionPanel GetMissionPanel()
        {
            return GetPanelInternal<MissionPanel>();
        }
        public DailyMissionMonitor GetDailyMissionMonitor()
        {
            return GetMissionPanel().GetBinding<DailyMissionMonitor>();
        }
    }
}
