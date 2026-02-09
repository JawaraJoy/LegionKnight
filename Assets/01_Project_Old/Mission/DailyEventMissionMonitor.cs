using UnityEngine;

namespace LegionKnight
{
    public class DailyEventMissionMonitor : MissionMonitor
    {
        private bool isInitiated = false;

        protected override MissionController GetControllerInternal()
        {
            if (m_Controller == null)
            {
                m_Controller = EventMissionManager.Instance.DailyEventMissionManager;
            }

            /*
            Debug.Log("xxxxxxxxxxxxxxxx");
            Debug.Log(m_Controller.Task.Length);

            foreach (var item in m_Controller.Task)
            {
                Debug.Log(item.Definition.Label);
            }
            */

            return m_Controller;
        }

        protected override void InitInternal(MissionController controller)
        {
            if(isInitiated)
                base.InitInternal(controller);
            else
                isInitiated = true;
        }
    }
}
