using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class WaitingView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_WaitingText;
        [SerializeField]
        private Button m_CloseButton;
        [SerializeField]
        private Button m_SuccessButton;
        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (m_WaitingText != null)
            {
                m_WaitingText.text = "Please Wait...";
            }
        }
        public void SetWaitingText(string text)
        {
            if (m_WaitingText != null)
            {
                m_WaitingText.text = text;
            }
        }
        public void ShowCloseButton(bool set)
        {
            if (m_CloseButton != null)
            {
                m_CloseButton.gameObject.SetActive(set);
            }
        }
        public void ShowSuccessButton(bool set)
        {
            if (m_SuccessButton != null)
            {
                m_SuccessButton.gameObject.SetActive(set);
            }
        }
    }
    public partial class AuthenticationPanel
    {
        private WaitingView GetWaitingView()
        {
            return GetBinding<WaitingView>();
        }
        public void ShowWaitingView()
        {
            var waitingView = GetWaitingView();
            if (waitingView != null)
            {
                waitingView.Show();
            }
        }
        public void ShowWaitingViewSuccessButton(bool set)
        {
            var waitingView = GetWaitingView();
            if (waitingView != null)
            {
                waitingView.ShowSuccessButton(set);
            }
        }
        public void ShowWaitingViewCloseButton(bool set)
        {
            var waitingView = GetWaitingView();
            if (waitingView != null)
            {
                waitingView.ShowCloseButton(set);
            }
        }
        public void SetWaitingText(string text)
        {
            var waitingView = GetWaitingView();
            if (waitingView != null)
            {
                waitingView.SetWaitingText(text);
            }
        }
        public void HideWaitingView()
        {
            var waitingView = GetWaitingView();
            if (waitingView != null)
            {
                waitingView.Hide();
            }
        }
    }

    public partial class GameManager
    {
        private AuthenticationPanel GetAuthenticationPanel()
        {
            return GetPanelInternal<AuthenticationPanel>();
        }
        public void ShowWaitingViewCloseButton(bool set)
        {
            var authenticationPanel = GetAuthenticationPanel();
            if (authenticationPanel != null)
            {
                authenticationPanel.ShowWaitingViewCloseButton(set);
            }
        }
        public void ShowWaitingViewSuccessButton(bool set)
        {
            var authenticationPanel = GetAuthenticationPanel();
            if (authenticationPanel != null)
            {
                authenticationPanel.ShowWaitingViewSuccessButton(set);
            }
        }
        public void ShowWaitingView()
        {
            var authenticationPanel = GetAuthenticationPanel();
            if (authenticationPanel != null)
            {
                authenticationPanel.ShowWaitingView();
            }
        }
        public void SetWaitingText(string text)
        {
            var authenticationPanel = GetAuthenticationPanel();
            if (authenticationPanel != null)
            {
                authenticationPanel.SetWaitingText(text);
            }
        }
        public void HideWaitingView()
        {
            var authenticationPanel = GetAuthenticationPanel();
            if (authenticationPanel != null)
            {
                authenticationPanel.HideWaitingView();
            }
        }
    }


}
