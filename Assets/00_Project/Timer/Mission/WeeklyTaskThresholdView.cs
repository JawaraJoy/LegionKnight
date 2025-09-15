using UnityEngine;

namespace LegionKnight
{
    public class WeeklyTaskThresholdView : TaskThresholdView
    {
        protected override MissionController GetControllerInternal()
        {
            return Player.Instance.WeeklyMissionManager;
        }
    }
}
