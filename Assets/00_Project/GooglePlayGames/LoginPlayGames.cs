using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.Events;

namespace LegionKnight
{
    public class LoginPlayGames : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<string> m_OnSignInSuccess;
        [SerializeField]
        private UnityEvent<string> m_OnSignInFailed;
        private void Awake()
        {
            PlayGamesPlatform.Activate();
        }
        private void Start()
        {
            SignIn();
        }

        private void SignIn()
        {
            
            PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways , ProcessAuthentication);
        }

        private void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success)
            {
                // Handle successful sign-in
                Debug.Log("Successfully signed in to Google Play Games.");
                string playerId = PlayGamesPlatform.Instance.GetUserId();
                string playerName = PlayGamesPlatform.Instance.GetUserDisplayName();
                m_OnSignInSuccess?.Invoke($"Signed in as {playerName} with ID {playerId}.");
            }
            else
            {
                // Handle sign-in failure
                Debug.LogError($"Failed to sign in to Google Play Games: {status}");
                string errorMessage = status switch
                {
                    SignInStatus.UiSignInRequired => "User interaction is required to sign in.",
                    SignInStatus.DeveloperError => "There is a configuration error with the Google Play Games setup.",
                    SignInStatus.NetworkError => "A network error occurred during sign-in.",
                    SignInStatus.InternalError => "An internal error occurred during sign-in.",
                    SignInStatus.Canceled => "The sign-in was canceled by the user.",
                    SignInStatus.AlreadyInProgress => "A sign-in process is already in progress.",
                    SignInStatus.Failed => "The sign-in failed for an unknown reason.",
                    _ => "An unknown error occurred."
                };
                m_OnSignInFailed?.Invoke($"Sign-in failed: {errorMessage}");
            }
        }
    }

    public partial class GooglePlayGames
    {
        [SerializeField]
        private LoginPlayGames m_LoginPlayGames;
    }
}
