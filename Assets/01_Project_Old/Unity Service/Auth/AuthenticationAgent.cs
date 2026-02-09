using System.Collections;
using System.Net;
using UnityEngine;

namespace LegionKnight
{
    public partial class AuthenticationAgent : MonoBehaviour
    {
        public void StartSinginWithUnity()
        {
            UnityService.Instance.StartSinginWithUnity();
        }
        public void SignInAnonymously()
        {
            UnityService.Instance.SignInAnonymously();
        }
        public void SignOut()
        {
            UnityService.Instance.SignOut();
        }
        public void ShowWaitingView()
        {
            CanvasManager.Instance.ShowWaitingView();
        }
        public void HideWaitingView()
        {
            CanvasManager.Instance.HideWaitingView();
        }
        public void SetWaitingText(string text)
        {
            CanvasManager.Instance.SetWaitingText(text);
        }
        public void ShowWaitingViewCloseButton(bool set)
        {
            CanvasManager.Instance.ShowWaitingViewCloseButton(set);
        }
        public void ShowWaitingViewSuccessButton(bool set)
        {
            CanvasManager.Instance.ShowWaitingViewSuccessButton(set);
        }

        private AuthenticationPanel GetAuthenticationPanel()
        {
            return CanvasManager.Instance.GetPanel<AuthenticationPanel>();
        }
        public void ShowAuthPanel()
        {
            GetAuthenticationPanel().Show();
        }
        public void HideAuthPanel(float wait)
        {
            StartCoroutine(WaitToHide(wait));
        }

        private IEnumerator WaitToHide(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            GetAuthenticationPanel().Hide();
        }
    }
}
