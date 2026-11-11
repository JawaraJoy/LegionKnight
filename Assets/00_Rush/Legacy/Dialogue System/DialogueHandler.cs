using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight.Dialogue
{
    public class DialogueHandler : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private ConversationDefinition m_SelectedConversation;
        [SerializeField, ReadOnly]
        private int m_SelectedDialogueIndex = -1;

        [SerializeField]
        private UnityEvent<ConversationDefinition> m_OnConversationStart;
        [SerializeField]
        private UnityEvent<ConversationDefinition> m_OnConversationEnd;
        [SerializeField]
        private UnityEvent<Dialogue> m_OnDialogueStart;
        [SerializeField]
        private UnityEvent<Dialogue> m_OnDialogueEnd;

        private Dialogue GetDialogueInternal(int index)
        {
            return m_SelectedConversation.Dialogues[index];
        }
        public void StartConversation(ConversationDefinition conversation)
        {
            m_SelectedConversation = conversation;
            m_SelectedDialogueIndex = 0;
            Dialogue firstDialogue = GetDialogueInternal(m_SelectedDialogueIndex);
            firstDialogue.OnDialogueStart.Invoke();
            m_SelectedConversation.OnConversationStart.Invoke(firstDialogue);
            m_OnConversationStart?.Invoke(m_SelectedConversation);
            m_OnDialogueStart?.Invoke(firstDialogue);
            Debug.Log($"Started conversation: {m_SelectedConversation.name}");
        }
        public void NextConversatioon()
        {
            Dialogue currentDialogue = GetDialogueInternal(m_SelectedDialogueIndex);
            currentDialogue.OnDialogueEnd.Invoke();
            m_OnDialogueEnd?.Invoke(currentDialogue);

            m_SelectedDialogueIndex++;
            Dialogue nextDialogue = GetDialogueInternal(m_SelectedDialogueIndex);
            nextDialogue.OnDialogueStart.Invoke();
            m_OnDialogueStart?.Invoke(nextDialogue);
        }

        public void EndConversation()
        {
            Dialogue lastDialogue = GetDialogueInternal(m_SelectedDialogueIndex);
            if (lastDialogue == null) return;

            lastDialogue.OnDialogueEnd.Invoke();
            m_OnDialogueEnd?.Invoke(lastDialogue);
            m_SelectedConversation.OnConversationEnd.Invoke(lastDialogue);
            m_OnConversationEnd?.Invoke(m_SelectedConversation);
            Debug.Log($"Ended conversation: {m_SelectedConversation.name}");
            m_SelectedConversation = null;
            m_SelectedDialogueIndex = -1;
        }
    }
}
