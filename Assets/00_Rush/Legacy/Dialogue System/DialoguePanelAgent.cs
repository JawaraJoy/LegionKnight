using UnityEngine;

namespace LegionKnight.Dialogue
{
    public class DialoguePanelAgent : MonoBehaviour
    {
        private DialoguePanel GetDialoguePanel()
        {
            DialoguePanel panel = CanvasManager.Instance.GetPanel<DialoguePanel>();
            return panel;
        }
        public void SetDialogue(Dialogue dialogue)
        {
            DialoguePanel panel = GetDialoguePanel();
            if (panel != null)
            {
                panel.SetDialogue(dialogue);
            }
            else
            {
                Debug.LogError("DialoguePanel not found!");
            }
        }
        public void Show()
        {
            DialoguePanel panel = GetDialoguePanel();
            if (panel != null)
            {
                panel.Show();
            }
            else
            {
                Debug.LogError("DialoguePanel not found!");
            }
        }
        public void Hide()
        {
            DialoguePanel panel = GetDialoguePanel();
            if (panel != null)
            {
                panel.Hide();
            }
            else
            {
                Debug.LogError("DialoguePanel not found!");
            }
        }
    }
}
