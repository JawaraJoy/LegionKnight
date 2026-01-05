using UnityEngine;

namespace LegionKnight
{
    public class DailyEventMissionView : MissionView
    {
        protected override MissionController GetControllerInternal()
        {
            return EventMissionManager.Instance.DailyEventMissionManager;
        }
    }
}
