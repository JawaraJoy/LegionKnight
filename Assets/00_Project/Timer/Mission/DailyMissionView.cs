using UnityEngine;

namespace LegionKnight
{
    public class DailyMissionView : MissionView
    {
        protected override MissionController GetControllerInternal()
        {
            return Player.Instance.DailyMissionManager;
        }
    }
}
