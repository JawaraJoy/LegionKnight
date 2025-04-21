using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System;

namespace LegionKnight
{
    public partial class Authentication : MonoBehaviour
    {
        async void Start()
        {
            // Initialize the authentication system
            await UnityServices.InitializeAsync();
        }
        public async void SignInAnonymously()
        {
            try
            {
                // Sign in anonymously
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Signed in anonymously {AuthenticationService.Instance.PlayerId}");
            }
            catch (AuthenticationException e)
            {
                Debug.LogError($"Failed to sign in: {e}");
            }
        }
    }
}
