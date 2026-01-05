using UnityEngine;

namespace LegionKnight
{
    public class WeeklyEventMissionView : MissionView
    {
        protected override MissionController GetControllerInternal()
        {
            return EventMissionManager.Instance.WeeklyEventMissionManager;
        }
    }
}
