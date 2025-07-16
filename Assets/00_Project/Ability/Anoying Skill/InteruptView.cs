using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class InteruptView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_InteruptText;

        public void SetInteruptText(int current, int max)
        {
            if (m_InteruptText != null)
            {
                m_InteruptText.text = $"Perfect to Interupt: {current}/{max}";
            }
            else
            {
                Debug.LogWarning("InteruptText is not assigned in the inspector.");
            }
        }
    }
    public partial class BosBarGameplay
    {
        [SerializeField]
        private InteruptView m_InteruptView;

        public void ShowInteruptView()
        {
            if (m_InteruptView != null)
            {
                m_InteruptView.Show();
            }
            else
            {
                Debug.LogWarning("InteruptView is not assigned in the inspector.");
            }
        }
        public void HideInteruptView()
        {
            if (m_InteruptView != null)
            {
                m_InteruptView.Hide();
            }
            else
            {
                Debug.LogWarning("InteruptView is not assigned in the inspector.");
            }
        }
        public void SetInteruptText(int current, int max)
        {
            if (m_InteruptView != null)
            {
                m_InteruptView.SetInteruptText(current, max);
            }
            else
            {
                Debug.LogWarning("InteruptView is not assigned in the inspector.");
            }
        }
    }

    public partial class GameplayPanel
    {
        public void SetInteruptText(int current, int max)
        {
            GetBosBar().SetInteruptText(current, max);
        }
        public void ShowInteruptView()
        {
            GetBosBar().ShowInteruptView();
        }
        public void HideInteruptView()
        {
            GetBosBar().HideInteruptView();
        }
    }
}
