using UnityEngine;

namespace LegionKnight
{
    public class WeeklyMissionMonitor : MissionMonitor
    {
        protected override MissionController GetControllerInternal()
        {
            if (m_Controller == null)
            {
                m_Controller = Player.Instance.WeeklyMissionManager;
            }
            return m_Controller;
        }
    }
    public partial class GameManager
    {
        public WeeklyMissionMonitor GetWeeklyMissionMonitor()
        {
            return GetMissionPanel().GetBinding<WeeklyMissionMonitor>();
        }
    }
}
