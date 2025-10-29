using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
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
        [SerializeField]
        private bool m_SignInOnStart;

        private string m_GooglePlayToken;
        private string m_GooglePlayEror;
        public string GooglePlayToken => m_GooglePlayToken;
        public string GooglePlayEror => m_GooglePlayEror;
        private void Awake()
        {
            PlayGamesPlatform.Activate();
        }
        private void Start()
        {
            if (m_SignInOnStart)
            {
                StartSignInWithGoogleInternal();
            }
        }
        private void StartSignInWithGoogleInternal()
        {
            Debug.Log("Starting sign-in with Google Play Games...");

            m_OnSignInStarted?.Invoke();
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        }
        public void StartSignInWithGoogle()
        {
            StartSignInWithGoogleInternal();
        }

        private void ProcessAuthentication(SignInStatus signInStatus)
        {
            /*switch (signInStatus)
            {
                case SignInStatus.Success:
                    // Sign-in was successful
                    //SignIn();
                    GoogleplayRequestToken();
                    break;
                case SignInStatus.InternalError:
                    // Sign-in failed
                    m_OnSignInFailed?.Invoke("Sign-in failed.");
                    Debug.LogError("Sign-in failed.");
                    break;
                case SignInStatus.Canceled:
                    // Sign-in was canceled by the user
                    m_OnSignInFailed?.Invoke("Sign-in canceled by user.");
                    Debug.LogWarning("Sign-in canceled by user.");
                    break;
                default:
                    
                    break;
            }*/
            if (signInStatus == SignInStatus.Success)
            {
                GoogleplayRequestToken();
            }
            else
            {
                m_GooglePlayEror = $"Unknown sign-in status: {signInStatus}";
                m_OnSignInFailed?.Invoke($"Failed to Login, sign-in status: " + signInStatus);
                Debug.LogError("Unknown sign-in status: " + signInStatus);
            }
        }

        private void GoogleplayRequestToken()
        {
            Debug.Log($"Start Request Token");
            PlayGamesPlatform.Instance.RequestServerSideAccess(true, SetGooglePlayToken);
        }

        private async void SetGooglePlayToken(string token)
        {
            m_GooglePlayToken = token;
            if (!string.IsNullOrEmpty(token))
            {
                await GooglePlayAuthenticateWithUnity(token); // fire and forget async
            }
            else
            {
                Debug.LogError($"Unknown sign-in status: {token}");
                m_OnSignInFailed?.Invoke("Failed to get server auth code.");
            }
        }

        private async Task GooglePlayAuthenticateWithUnity(string token)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(token);
                m_OnSignInSuccess?.Invoke(PlayGamesPlatform.Instance.GetUserDisplayName());
                Debug.Log($"Success Sign in {token}");
            }
            catch (AuthenticationException e)
            {
                m_OnSignInFailed?.Invoke(e.Message);
                Debug.LogError(e);
                throw;
            }
            catch(RequestFailedException ec)
            {
                Debug.LogError(ec);
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
