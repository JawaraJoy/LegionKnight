using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight.Dialogue
{
    [CreateAssetMenu(fileName = "New Conversation", menuName = "Legion Knight/Dialogue/Conversation", order = 1)]
    public class ConversationDefinition : ScriptableObject
    {
        [SerializeField]
        private Dialogue[] m_Dialogues;

        [SerializeField]
        private UnityEvent<Dialogue> m_OnConversationStart;
        [SerializeField]
        private UnityEvent<Dialogue> m_OnConversationEnd;

        public Dialogue[] Dialogues => m_Dialogues;
        public UnityEvent<Dialogue> OnConversationStart => m_OnConversationStart;
        public UnityEvent<Dialogue> OnConversationEnd => m_OnConversationEnd;

        public void StartConversation()
        {
            GameManager.Instance.StartConversation(this);
        }
        public void NextConversation()
        {
            GameManager.Instance.NextConversatioon();
        }
    }
}
