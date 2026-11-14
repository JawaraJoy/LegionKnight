using LegionKnight.Dialogue;
using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Tutor Step", menuName = "Legion Knight/Tutorial/Tutor Step")]
    public partial class TutorStepDefinition : ScriptableObject
    {
        [SerializeField] private string m_Id;
        [SerializeField] private string m_Title;
        [SerializeField] private ConversationDefinition m_Conversation;
        [SerializeField] private UnityEvent m_OnStepStart = new();
        [SerializeField] private UnityEvent m_OnStepEnd = new();
        [SerializeField] private TutorialAction m_CompletionAction;
        [SerializeField] private bool m_RequiresTarget = true;
        [SerializeField] private bool m_PreserveSuccess;
        [SerializeField] private bool m_SkipCompletedOnResume;
        [SerializeField] private TutorStepDefinition m_SuccessDependency;
        [SerializeField] private TutorStepDefinition m_ResumeSuccessDependency;
        [SerializeField] private LootChestDefinition m_Reward;
        public string Id => m_Id;
        public string Title => m_Title;
        public ConversationDefinition Conversation => m_Conversation;
        public bool HasConversation => m_Conversation != null;
        public UnityEvent OnStepStart => m_OnStepStart;
        public UnityEvent OnStepEnd => m_OnStepEnd;
        public TutorialAction CompletionAction => m_CompletionAction;
        public bool RequiresTarget => m_RequiresTarget;
        public bool PreserveSuccess => m_PreserveSuccess;
        public bool SkipCompletedOnResume => m_SkipCompletedOnResume;
        public TutorStepDefinition SuccessDependency => m_SuccessDependency;
        public TutorStepDefinition ResumeSuccessDependency => m_ResumeSuccessDependency;
        public LootChestDefinition Reward => m_Reward;
        public void NotifyStarted() => OnStepStartInvoke();
        public void NotifyEnded() => OnStepEndInvoke();
        private void OnStepStartInvoke() => m_OnStepStart?.Invoke();
        private void OnStepEndInvoke() => m_OnStepEnd?.Invoke();
    }
}
