using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class GoogleAuth : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent m_OnSignInStarted;
        [SerializeField]
        private UnityEvent<string> m_OnSignInSuccess;
        [SerializeField]
        private UnityEvent<string> m_OnSignInFailed;

        private string m_AccountName;
        private string m_Id;
        private string m_ImageUrl;

        private void Awake()
        {
            PlayGamesPlatform.Activate();
        }
        public void StartSignInWithGoogle()
        {
            // This method should contain the logic to start the sign-in process with Google Play Games.
            // For example, you might call a method from a Google Play Games SDK to initiate the sign-in.
            Debug.Log("Starting sign-in with Google Play Games...");
            // Implement the actual sign-in logic here.
            
            m_OnSignInStarted.Invoke();
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        }

        private void ProcessAuthentication(SignInStatus signInStatus)
        {
            switch (signInStatus)
            {
                case SignInStatus.Success:
                    // Sign-in was successful
                    m_AccountName = PlayGamesPlatform.Instance.GetUserDisplayName();
                    m_Id = PlayGamesPlatform.Instance.GetUserId();
                    m_ImageUrl = PlayGamesPlatform.Instance.GetUserImageUrl();
                    m_OnSignInSuccess.Invoke($"Login Success {m_AccountName}");
                    Debug.Log("Sign-in successful. Account Name: " + m_AccountName + ", ID: " + m_Id + ", Image URL: " + m_ImageUrl);
                    break;
                case SignInStatus.InternalError:
                    // Sign-in failed
                    m_OnSignInFailed.Invoke("Sign-in failed.");
                    Debug.LogError("Sign-in failed.");
                    break;
                case SignInStatus.Canceled:
                    // Sign-in was canceled by the user
                    m_OnSignInFailed.Invoke("Sign-in canceled by user.");
                    Debug.LogWarning("Sign-in canceled by user.");
                    break;
                default:
                    m_OnSignInFailed.Invoke("Unknown sign-in status: " + signInStatus);
                    Debug.LogError("Unknown sign-in status: " + signInStatus);
                    break;
            }
        }
    }

    public partial class GooglePlayService
    {
        [SerializeField]
        private GoogleAuth m_Authentication;

        public void StartSignInWithGoogle()
        {
            m_Authentication.StartSignInWithGoogle();
        }
    }
}
