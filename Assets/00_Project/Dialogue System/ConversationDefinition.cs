using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class ConversationDefinition : ScriptableObject
    {
        [SerializeField]
        private DialogueDefinition[] m_Dialogues;

        [SerializeField]
        private UnityEvent<DialogueDefinition> m_OnConversationStart;
        [SerializeField]
        private UnityEvent<DialogueDefinition> m_OnConversationEnd;

        public DialogueDefinition[] Dialogues => m_Dialogues;
        public UnityEvent<DialogueDefinition> OnConversationStart => m_OnConversationStart;
        public UnityEvent<DialogueDefinition> OnConversationEnd => m_OnConversationEnd;
    }
}
