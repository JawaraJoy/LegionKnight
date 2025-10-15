using UnityEngine;
using LegionKnight.Dialogue;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Tutor Step", menuName = "Legion Knight/Tutorial/Tutor Step")]
    public partial class TutorStepDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_Title;
        [SerializeField]
        private ConversationDefinition m_Conversation;
        public string Id => m_Id;
        public string Title => m_Title;
        public ConversationDefinition Conversation => m_Conversation;

        public bool HasConversation => m_Conversation != null;
    }
}
