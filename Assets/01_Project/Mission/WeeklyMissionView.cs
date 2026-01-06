using UnityEngine;

namespace LegionKnight
{
    public class WeeklyMissionView : MissionView
    {
        protected override MissionController GetControllerInternal()
        {
            return Player.Instance.WeeklyMissionManager;
        }
    }
}
