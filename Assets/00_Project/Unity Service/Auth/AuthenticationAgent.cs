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
            GameManager.Instance.ShowWaitingView();
        }
        public void HideWaitingView()
        {
            GameManager.Instance.HideWaitingView();
        }
        public void SetWaitingText(string text)
        {
            GameManager.Instance.SetWaitingText(text);
        }
        public void ShowWaitingViewCloseButton(bool set)
        {
            GameManager.Instance.ShowWaitingViewCloseButton(set);
        }
        public void ShowWaitingViewSuccessButton(bool set)
        {
            GameManager.Instance.ShowWaitingViewSuccessButton(set);
        }
    }
}
