using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class CompleteTab : UIView
    {
        private DownloadContent m_DownloadContent;

        private DownloadContent GetDownloadContent()
        {
            if (m_DownloadContent == null)
            {
                m_DownloadContent = UnityService.Instance.GetDownloadContent();
            }
            return m_DownloadContent;
        }

        [SerializeField]
        private Button m_ContinueButton;

        private void Awake()
        {
            m_ContinueButton.onClick.AddListener(OnContinueButtonPressed);
        }

        private void OnContinueButtonPressed()
        {
            GetDownloadContent().Continue();
            HideInternal();
        }
    }

    public partial class DownloadContent
    {
        private CompleteTab m_CompleteTab;
        private CompleteTab GetCompleteTab()
        {
            if (m_CompleteTab == null)
            {
                m_CompleteTab = GetDownloadPanel().GetBinding<CompleteTab>();
            }
            return m_CompleteTab;
        }
    }
}
