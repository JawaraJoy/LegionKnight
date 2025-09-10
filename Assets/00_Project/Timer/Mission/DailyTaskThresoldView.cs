using UnityEngine;

namespace LegionKnight
{
    public class DailyTaskThresoldView : TaskThresholdView
    {
        protected override MissionController GetControllerInternal()
        {
            return Player.Instance.DailyMissionManager;
        }
    }
}
