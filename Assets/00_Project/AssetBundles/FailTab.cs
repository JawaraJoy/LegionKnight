using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class FailTab : UIView
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
        private Button m_QuitButton;

        private void Awake()
        {
            m_QuitButton.onClick.AddListener(Quit);
        }

        private void Quit()
        {
            Application.Quit();
            HideInternal();
        }
    }

    public partial class DownloadContent
    {
        private FailTab m_FailTab;
        private FailTab GetFailTab()
        {
            if (m_FailTab == null)
            {
                m_FailTab = GetDownloadPanel().GetBinding<FailTab>();
            }
            return m_FailTab;
        }
    }
}
