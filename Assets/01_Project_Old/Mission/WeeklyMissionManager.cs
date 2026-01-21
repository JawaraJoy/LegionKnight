using UnityEngine;

namespace LegionKnight
{
    public partial class WeeklyMissionManager : MissionController
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private WeeklyMissionManager m_WeeklyMissionManager;
        public WeeklyMissionManager WeeklyMissionManager => m_WeeklyMissionManager;
    }
}
