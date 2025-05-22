using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        private Button m_SkipButton;
        public override string UniqueId => PanelId.Tutorial;

        public void PlayTutor(MaskingTarget target)
        {
            ShowInternal();
            m_InvertMaskingAuto.SetMaskingTarget(target);
            
            int descriptionIndex = GameManager.Instance.TutorDescriptionIndex;
            SetDialogue(target.DialogueTitle, target.DialogueDescriptions[descriptionIndex]);
            m_SkipButton.gameObject.SetActive(target.DialogueDefinition.CanSkip);
        }

        public void EndTutor()
        {
            HideInternal();
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
                tutorialPanel.EndTutor();
            }
            else
            {
                Debug.LogError("Tutorial panel not found.");
            }
        }

    }
}
