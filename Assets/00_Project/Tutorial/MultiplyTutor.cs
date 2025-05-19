using UnityEngine;

namespace LegionKnight
{
    public partial class MultiplyTutor : MonoBehaviour
    {
        [SerializeField]
        private MaskingTarget[] m_TutorialTarget;

        private DialogueDefinition GetDialogueById(string id)
        {
            foreach (MaskingTarget target in m_TutorialTarget)
            {
                if (target.DialogueId == id)
                {
                    return target.DialogueDefinition;
                }
            }
            Debug.LogError($"No dialogue found with ID: {id}");
            return null;
        }
        private bool IsTutorialCompleted(string id)
        {
            foreach (MaskingTarget target in m_TutorialTarget)
            {
                if (target.DialogueId == id)
                {
                    return target.IsTutorialCompleted;
                }
            }
            Debug.LogError($"No tutorial found with ID: {id}");
            return false;
        }
        public void StartTutor()
        {
            foreach (MaskingTarget target in m_TutorialTarget)
            {
                if (!target.IsTutorialCompleted)
                {
                    target.StartDialogue();
                    return;
                }
            }
        }
    }
}
