using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public static partial class  PanelId
    {
        public const string Tutorial = "Tutorial";
    }
    public partial class TutorialPanel : PanelView
    {
        [SerializeField]
        private InvertMaskingAuto m_InvertMaskingAuto = null;
        [SerializeField]
        private TextMeshProUGUI m_TitleText;
        [SerializeField]
        private TextMeshProUGUI m_DescriptionText;
        [SerializeField]
        public override string UniqueId => PanelId.Tutorial;

        public void PlayTutor(MaskingTarget target)
        {
            m_InvertMaskingAuto.SetMaskingTarget(target);
            
            int descriptionIndex = GameManager.Instance.TutorDescriptionIndex;
            SetDialogue(target.DialogueTitle, target.DialogueDescriptions[descriptionIndex]);
            ShowInternal();
            
        }

        // call on tutormanager
        public void EndDialogue()
        {
            
            HideInternal();
            GameManager.Instance.EndTutorDialogue();
        }

        private void SetDialogue(string title, string description)
        {
            m_TitleText.text = title;
            m_DescriptionText.text = description;
        }
    }

    public partial class GameManager
    {
        private TutorialPanel GetTutorialPanelInternal()
        {
            return GetPanel<TutorialPanel>();
        }
        public void PlayTutorialPanel(MaskingTarget target)
        {
            var tutorialPanel = GetTutorialPanelInternal();
            if (tutorialPanel != null)
            {
                tutorialPanel.PlayTutor(target);
            }
            else
            {
                Debug.LogError("Tutorial panel not found.");
            }
        }
        public void EndTutorialPanel()
        {
            var tutorialPanel = GetTutorialPanelInternal();
            if (tutorialPanel != null)
            {
                tutorialPanel.EndDialogue();
            }
            else
            {
                Debug.LogError("Tutorial panel not found.");
            }
        }

    }
}
