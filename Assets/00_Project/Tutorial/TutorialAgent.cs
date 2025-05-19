using UnityEngine;

namespace LegionKnight
{
    public partial class TutorialAgent : MonoBehaviour
    {
        public void Init()
        {
            GameManager.Instance.InitTutorial();
        }
        public void StartTutorial(string id)
        {
            GameManager.Instance.StartTutorial(id);
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
        public void SetCanTutor(string id, bool canTutor)
        {
            GameManager.Instance.SetCanTutor(id, canTutor);
        }
    }
}
