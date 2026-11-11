using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight.Dialogue
{
    [System.Serializable]
    public class ConversationEvent
    {
        [SerializeField]
        private ConversationDefinition m_Defi;

        public ConversationDefinition Definition => m_Defi;

        [SerializeField]
        private UnityEvent<ConversationDefinition> m_OnConversationStart;
        [SerializeField]
        private UnityEvent<ConversationDefinition> m_OnConversationEnd;

        public void OnConversationStartInvoke(ConversationDefinition conversation)
        {
            if (conversation == m_Defi)
            {
                m_OnConversationStart.Invoke(conversation);
            }
        }
        public void OnConversationEndInvoke(ConversationDefinition conversation)
        {
            if (conversation == m_Defi)
            {
                m_OnConversationEnd.Invoke(conversation);
            }
        }
    }
    public class DialogueEventTrigger : MonoBehaviour
    {
        [SerializeField]
        private ConversationEvent[] m_ConversationEvents;

        private ConversationDefinition GetConversation(ConversationDefinition conversation)
        {
            foreach (var de in m_ConversationEvents)
            {
                if (de.Definition == conversation)
                {
                    return de.Definition;
                }
            }
            return null;
        }
        public void OnConversationStartInvoke(ConversationDefinition conversation)
        {
            var def = GetConversation(conversation);
            if (def != null)
            {
                foreach (var de in m_ConversationEvents)
                {
                    de.OnConversationStartInvoke(def);
                }
            }
        }
        public void OnConversationEndInvoke(ConversationDefinition conversation)
        {
            var def = GetConversation(conversation);
            if (def != null)
            {
                foreach (var de in m_ConversationEvents)
                {
                    de.OnConversationEndInvoke(def);
                }
            }
        }
    }
}
