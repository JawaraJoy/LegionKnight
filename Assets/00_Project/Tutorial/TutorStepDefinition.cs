using LegionKnight.Dialogue;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

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
        [SerializeField]
        private UnityEvent m_OnStepStart;
        [SerializeField]
        private UnityEvent m_OnStepEnd;
        public string Id => m_Id;
        public string Title => m_Title;
        
        public ConversationDefinition Conversation => m_Conversation;
        public bool HasConversation => m_Conversation != null;
        public UnityEvent OnStepStart => m_OnStepStart;
        public UnityEvent OnStepEnd => m_OnStepEnd;
        
    }
}
