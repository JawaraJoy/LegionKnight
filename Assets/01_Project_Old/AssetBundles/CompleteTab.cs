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
        private DownloadPanel m_Panel;
        private DownloadPanel Panel
        {
            get
            {
                if (m_Panel == null)
                {
                    m_Panel = CanvasManager.Instance.GetPanel<DownloadPanel>();
                }
                return m_Panel;
            }
        }
        private void OnContinueButtonPressed()
        {
            GetDownloadContent().OnContinueAfterSuccessPublic.Invoke();
            HideInternal();
            Panel.Hide();
        }
    }
}
