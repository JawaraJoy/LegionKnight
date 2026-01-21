using UnityEngine;

namespace LegionKnight
{
    public class EventMissionAgent : MonoBehaviour
    {
        private EventMissionManager eventMissionManager;

        public void Init()
        {
            eventMissionManager = GameObject.FindFirstObjectByType<EventMissionManager>();
            if(eventMissionManager)
                eventMissionManager.Init();
        }
    }
}
