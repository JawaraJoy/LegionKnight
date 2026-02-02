using UnityEngine;

namespace LegionKnight
{
    public class EventMissionManager : Singleton<EventMissionManager>
    {
        [SerializeField]
        private DailyEventManager m_DailyEventMissionManager;
        public DailyEventManager DailyEventMissionManager => m_DailyEventMissionManager;

        [SerializeField]
        private WeeklyEventManager m_WeeklyEventMissionManager;
        public WeeklyEventManager WeeklyEventMissionManager => m_WeeklyEventMissionManager;


        public void Init()
        {
            if (m_DailyEventMissionManager)
                m_DailyEventMissionManager.InitController();

            if (m_WeeklyEventMissionManager)
                m_WeeklyEventMissionManager.InitController();
        }
    }
}
