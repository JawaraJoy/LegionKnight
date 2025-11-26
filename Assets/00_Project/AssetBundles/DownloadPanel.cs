using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class DownloadPanel : PanelView
    {
        [SerializeField]
        private TextMeshProUGUI m_DescriptionText;
        [SerializeField]
        private Slider m_ProgressBar;

        [SerializeField]
        private GameObject m_Confirmation;
        [SerializeField]
        private GameObject m_DownloadProgress;

        public void OpenConfirmation(long sizeDownload)
        {
            SetDescriptionInternal(sizeDownload);
            m_Confirmation.SetActive(true);
        }

        public void OpenUpdateProgress()
        {
            
        }

        private void SetDescriptionInternal(long sizeDownload)
        {
            string description = $"Additional content needs to be downloaded to continue. The download size is approximately {sizeDownload / (1024 * 1024)} MB.";
            m_DescriptionText.text = description;
        }
        public void SetDescription(long sizeDownload)
        {
            SetDescriptionInternal(sizeDownload);
        }
        public void SetDescription(string description)
        {
            m_DescriptionText.text = description;
        }
    }
}
