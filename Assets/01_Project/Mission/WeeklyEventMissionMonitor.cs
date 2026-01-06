using UnityEngine;

namespace LegionKnight
{
    public class WeeklyEventMissionMonitor : MissionMonitor
    {
        protected override MissionController GetControllerInternal()
        {
            if (m_Controller == null)
            {
                m_Controller = EventMissionManager.Instance.WeeklyEventMissionManager;
            }
            return m_Controller;
        }
    }
}
