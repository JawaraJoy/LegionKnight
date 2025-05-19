using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class TutorialManager : TutorialHandler
    {
        
    }

    public partial class GameManager
    {
        [SerializeField]
        private TutorialManager m_TutorialManager;
        public MaskingTarget MaskingTarget => m_TutorialManager.CurrentMaskingTarget;
        public void StartTutorial(string id)
        {
            m_TutorialManager.StartTutorial(id);
        }
        public int TutorDescriptionIndex => m_TutorialManager.DescriptionIndex;

        public void NextTutorialDialogue()
        {
            m_TutorialManager.NextDialogue();
        }

        public void EndTutorDialogue()
        {
            m_TutorialManager.EndDialogue();
        }
        public void InitTutorial()
        {
            m_TutorialManager.Init();
        }
        public void SetCanTutor(string id, bool canTutor)
        {
            m_TutorialManager.SetCanTutor(id, canTutor);
        }
    }
}
