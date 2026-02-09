using UnityEngine;

namespace LegionKnight
{
    public class EventMissionAgent : MonoBehaviour
    {
        private EventMissionManager eventMissionManager;

        public void Init()
        {
            eventMissionManager = EventMissionManager.Instance;
            if (eventMissionManager)
                eventMissionManager.Init();
        }
    }
}
