using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class Authentication : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent m_OnStartSignIn;
        [SerializeField]
        private UnityEvent<string> m_OnSignInSuccess;
        [SerializeField]
        private UnityEvent<string> m_OnSignInFailed;
        private async void Start()
        {
            // Initialize the authentication system
            await UnityServices.InitializeAsync();
            // Register the Unity Player Accounts sign-in event handler after services initialization.
            PlayerAccountService.Instance.SignedIn += StartSinginWithUnity;
        }

        public async void StartSinginWithUnity()
        {
            if (PlayerAccountService.Instance.IsSignedIn)
            {
                // If the player is already signed into Unity Player Accounts, proceed directly to the Unity Authentication sign-in.
                await SingInWithUnity();
                return;
            }

            try
            {
                // This will open the system browser and prompt the user to sign in to Unity Player Accounts
                await PlayerAccountService.Instance.StartSignInAsync();
            }
            catch (PlayerAccountsException ex)
            {
                // Compare error code to PlayerAccountsErrorCodes
                // Notify the player with the proper error message
                OnSignInFailedInvoke($"Failed to sign in: {ex.Message}");
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                OnSignInFailedInvoke($"Failed to sign in: {ex.Message}");
                Debug.LogException(ex);
            }
        }
        private async Task SingInWithUnity()
        {
            OnStartSingInInvoke();
            try
            {
                await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
                OnSignInSuccessInvoke($"Signed in with Unity {PlayerAccountService.Instance.AccessToken} is Successed");
                Debug.Log("SignIn is successful.");
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
                OnSignInFailedInvoke($"Failed to sign in: {ex.Message}");
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
                OnSignInFailedInvoke($"Failed to sign in: {ex.Message}");
            }
        }
        private async Task LinkWithUnityAsync(string accessToken)
        {
            try
            {
                await AuthenticationService.Instance.LinkWithUnityAsync(accessToken);
                Debug.Log("Link is successful.");
            }
            catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
            {
                // Prompt the player with an error message.
                Debug.LogError("This user is already linked with another account. Log in instead.");
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }
        async Task UnlinkUnityAsync()
        {
            try
            {
                await AuthenticationService.Instance.UnlinkUnityAsync();
                Debug.Log("Unlink is successful.");
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }
        public void SignOut(bool clearToken = false)
        {
            // Sign out of Unity Authentication, with the option to clear the session token
            AuthenticationService.Instance.SignOut(clearToken);

            // Sign out of Unity Player Accounts
            PlayerAccountService.Instance.SignOut();
        }
        public async void SignInAnonymously()
        {
            OnStartSingInInvoke();
            try
            {
                // Sign in anonymously
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                OnSignInSuccessInvoke($"Signed in anonymously {AuthenticationService.Instance.PlayerId} is Successed");
                Debug.Log($"Signed in anonymously {AuthenticationService.Instance.PlayerId}");
            }
            catch (AuthenticationException e)
            {
                Debug.LogError($"Failed to sign in: {e}");
                OnSignInFailedInvoke($"Failed to sign in: {e.Message}");
            }
        }
        private void OnStartSingInInvoke()
        {
            // This event is triggered when the sign-in process starts
            Debug.Log("Sign-in started");
            m_OnStartSignIn?.Invoke();
        }
        private void OnSignInSuccessInvoke(string playerId)
        {
            // This event is triggered when the sign-in is successful
            Debug.Log($"Sign-in successful: {playerId}");
            m_OnSignInSuccess?.Invoke(playerId);
        }
        private void OnSignInFailedInvoke(string error)
        {
            // This event is triggered when the sign-in fails
            Debug.Log($"Sign-in failed: {error}");
            m_OnSignInFailed?.Invoke(error);
        }
    }
}
