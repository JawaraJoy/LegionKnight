using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class ConfirmationTab : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_DescriptionText;

        [SerializeField]
        private Button m_DownloadButton;
        [SerializeField]
        private Button m_CancelButton;

        private void Awake()
        {
            m_CancelButton.onClick.AddListener(Cancel);
            m_DownloadButton.onClick.AddListener(Confirm);
        }
        private DownloadContent m_DownloadContent;

        private DownloadContent GetDownloadContent()
        {
            if (m_DownloadContent == null)
            {
                m_DownloadContent = UnityService.Instance.GetDownloadContent();
            }
            return m_DownloadContent;
        }
        public void SetDescription(long sizeDownload)
        {
            string description = $"Additional content needs to be downloaded to continue. The download size is approximately {sizeDownload / (1024 * 1024)} MB.";
            m_DescriptionText.text = description;
        }

        private void Confirm()
        {
            GetDownloadContent().ConfirmDownload();
            HideInternal();
        }
        private void Cancel()
        {
            GetDownloadContent().CancelDownload();
            Application.Quit();
        }

        private void OnDestroy()
        {
            
        }

    }
}
