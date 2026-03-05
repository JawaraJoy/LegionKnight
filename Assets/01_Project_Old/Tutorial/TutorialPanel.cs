using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    
    public partial class TutorialPanel : PanelView
    {
        [SerializeField]
        private InvertMaskingAuto m_InvertMaskingAuto = null;
        [SerializeField]
        private Button m_SkipButton;
        [SerializeField]
        private Button m_NextButton;
        public Button NextButton => m_NextButton;
        public void SetTutorial(TutorTarget target)
        {
            m_InvertMaskingAuto.SetMaskingTarget(target);
        }
        public void Refresh()
        {
            m_InvertMaskingAuto.Refresh();
        }
    }
}
