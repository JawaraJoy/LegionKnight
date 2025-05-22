using UnityEngine;

namespace LegionKnight
{
    public partial class TutorialAgent : MonoBehaviour
    {
        public void Init()
        {
            GameManager.Instance.InitTutorial();
        }
        public void StartTutorial(DialogueDefinition defi)
        {
            GameManager.Instance.StartTutorial(defi);
        }

        public void NextTutorialDialogue()
        {
            GameManager.Instance.NextTutorialDialogue();
        }

        public void EndTutorDialogue()
        {
            GameManager.Instance.EndTutorDialogue();
        }

        public void PlayTutorPanel(MaskingTarget target)
        {
            GameManager.Instance.PlayTutorialPanel(target);
        }
        public void EndTutorialPanel()
        {
            GameManager.Instance.EndTutorialPanel();
        }
        public void SetUnlockTutor(string id, bool unlock)
        {
            GameManager.Instance.SetUnlockTutor(id, unlock);
        }
    }
}
