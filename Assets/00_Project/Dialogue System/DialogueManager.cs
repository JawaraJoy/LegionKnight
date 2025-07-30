using UnityEngine;

namespace LegionKnight.Dialogue
{
    public class DialogueManager : DialogueHandler
    {
        
    }
}


namespace LegionKnight
{
    using LegionKnight.Dialogue;
    public partial class GameManager
    {
        [SerializeField]
        private DialogueManager m_DialogueManager;

        public void StartConversation(ConversationDefinition conversation)
        {
            m_DialogueManager.StartConversation(conversation);
        }
        public void NextConversatioon()
        {
            m_DialogueManager.NextConversatioon();
        }
        public void EndConversation()
        {
            m_DialogueManager.EndConversation();
        }
    }
}
