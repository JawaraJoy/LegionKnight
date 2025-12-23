using UnityEngine;

namespace LegionKnight
{
    public class EventMissionManager : MonoBehaviour
    {
        [SerializeField]
        private DailyEventManager m_DailyEventMissionManager;
        public DailyEventManager DailyEventMissionManager => m_DailyEventMissionManager;

        [SerializeField]
        private WeeklyEventManager m_WeeklyEventMissionManager;
        public WeeklyEventManager WeeklyEventMissionManager => m_WeeklyEventMissionManager;

        public static EventMissionManager Instance {get; private set;}

        public void Init()
        {
            if(!Instance)
            {
                Instance = this;

                if(m_DailyEventMissionManager)
                    m_DailyEventMissionManager.InitController();

                if(m_WeeklyEventMissionManager)
                    m_WeeklyEventMissionManager.InitController();
            }
        }
    }
}
