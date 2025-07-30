using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight.Dialogue
{
    public class DialoguePanel : PanelView
    {
        [SerializeField]
        private TextMeshProUGUI m_OwnerNameText;
        [SerializeField]
        private TextMeshProUGUI m_DescriptionText;
        [SerializeField]
        private TextMeshProUGUI m_ButtonText;

        [SerializeField]
        private Button m_DialogueButton;

        [SerializeField]
        private UnityEvent<Dialogue> m_OnDialogueEnd;
        public void SetDialogue(Dialogue dialogue)
        {
            m_OwnerNameText.text = dialogue.OwnerName;
            m_DescriptionText.text = dialogue.Description;
            string buttonText = dialogue.IsOver ? "Continue" : "Next";
            m_ButtonText.text = buttonText;
            m_DialogueButton.onClick.RemoveAllListeners();
            m_DialogueButton.onClick.AddListener(DialogueAction(dialogue));
        }
        private UnityAction DialogueAction(Dialogue dialogue)
        {
            return () =>
            {
                if (dialogue.IsOver)
                {
                    m_OnDialogueEnd.Invoke(dialogue);
                    End();
                }
                else
                {
                    Next();
                }
            };
        }
        private void Next()
        {
            GameManager.Instance.NextConversatioon();
        }
        private void End()
        {
            GameManager.Instance.EndConversation();
        }
    }
}
