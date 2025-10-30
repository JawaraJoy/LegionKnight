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
        private Button m_SkipButton;
        public override string UniqueId => PanelId.Tutorial;
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
