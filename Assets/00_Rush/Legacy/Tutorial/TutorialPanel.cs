using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class TutorialPanel : PanelView
    {
        [SerializeField] private InvertMaskingAuto m_InvertMaskingAuto;
        [SerializeField] private Button m_SkipButton;
        [SerializeField] private Button m_NextButton;
        private TutorialHandler m_Handler;
        public Button NextButton => m_NextButton;
        public void Bind(TutorialHandler handler)
        {
            if (m_SkipButton != null) m_SkipButton.onClick.RemoveListener(PauseInternal);
            m_Handler = handler;
            if (m_SkipButton != null) m_SkipButton.onClick.AddListener(PauseInternal);
        }
        private void PauseInternal() => m_Handler?.PauseTutorial();
        // Compatibility for serialized calls in older prefabs; pausing never marks completion.
        public void EndDialogue() => PauseInternal();
        public void SetTutorial(TutorTarget target) => m_InvertMaskingAuto?.SetMaskingTarget(target);
        public void Refresh() => m_InvertMaskingAuto?.Refresh();
        private void OnDestroy()
        {
            if (m_SkipButton != null) m_SkipButton.onClick.RemoveListener(PauseInternal);
        }
    }
}
