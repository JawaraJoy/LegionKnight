using UnityEngine;

namespace LegionKnight
{
    public class DailyMissionManager : MissionController
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private DailyMissionManager m_DailyMissionManager;
        public DailyMissionManager DailyMissionManager => m_DailyMissionManager;
    }
}
