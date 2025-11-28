using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class LoadingProgressTab : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_PercentageText;
        [SerializeField]
        private TextMeshProUGUI m_LogText;
        [SerializeField]
        private Slider m_ProgressBar;
        public void SetProgress(float progress)
        {
            m_ProgressBar.value = progress;
            m_PercentageText.text = $"{progress * 100f:0.00}%";
        }

        public void LogMessage(string message)
        {
            m_LogText.text = message + "\n";
        }
    }
}
