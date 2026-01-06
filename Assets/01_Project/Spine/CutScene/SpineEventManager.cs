using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class SpineEventManager : MonoBehaviour
    {
        [SerializeField]
        private SpineEventContainer[] m_Events;

        private SpineEventContainer GetSpineEventInternal(string id)
        {
            SpineEventContainer spineEvent = null;
            foreach (var e in m_Events)
            {
                if (e.EventDefinition.EventId == id)
                {
                    spineEvent = e;
                }
            }
            return spineEvent;
        }
        public bool HasSpineEvent(string eventName, out SpineEventContainer ev)
        {
            bool has = GetSpineEventInternal(eventName) != null;
            if (has)
            {
                ev = GetSpineEventInternal(eventName);
            }
            else
            {
                ev = null;
            }
            return has;
        }
    }

    [System.Serializable]
    public class SpineEventContainer
    {
        [SerializeField]
        private SpineEventDefinition m_EventDefintion;
        [SerializeField]
        private UnityEvent<GameObject> m_OnTriggered;
        public SpineEventDefinition EventDefinition => m_EventDefintion;
        public void OnTriggeredInvoke(GameObject obj)
        {
            m_OnTriggered?.Invoke(obj);
            Debug.Log($"[Spine Event]{m_EventDefintion.EventId} is triggered");
        }
    }

    public partial class GameManager
    {
        [SerializeField]
        private SpineEventManager m_SpineEventManager;
        public SpineEventManager SpineEventManager => m_SpineEventManager;
    }
}
