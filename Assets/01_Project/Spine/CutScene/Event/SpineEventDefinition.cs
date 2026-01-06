using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "SpineEvent", menuName = "Legion Knight/Spine/Spine Event")]
    public class SpineEventDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_EventId;
        [SerializeField]
        private string m_EventName;

        private GameObject m_EventSender;
        public string EventId => m_EventId;
        public string EventName => m_EventName;

        private SpineEventManager m_EventManager;

        private SpineEventManager GetSpineEventManager()
        {
            if (m_EventManager == null)
            {
                m_EventManager = GameManager.Instance.SpineEventManager;
            }
            return m_EventManager;
        }

        private void HandleSpineEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data.Name == m_EventName)
            {
                bool has = GetSpineEventManager().HasSpineEvent(m_EventId, out SpineEventContainer val);
                if (has)
                {
                    val.OnTriggeredInvoke(m_EventSender);
                }

            }
            Debug.Log($"[Spine Event] {e.Data.Name} at time {e.Time}");
        }
        public void AddEventCallBack(SkeletonGraphic anim, GameObject sender)
        {
            m_EventSender = sender;
            anim.AnimationState.Event += HandleSpineEvent;
        }
    }
}
