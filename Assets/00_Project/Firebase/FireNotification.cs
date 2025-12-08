using Firebase.Messaging;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace LegionKnight
{
    public class FireNotification : MonoBehaviour
    {
        private async void Start()
        {
            // Initialize Firebase
            await InitializeFirebase();
        }

        private async Task InitializeFirebase()
        {
            var dependencyStatus = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();

            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                Debug.Log("Firebase Ready!");

                // Register FCM events AFTER initialization
                FirebaseMessaging.TokenReceived += OnTokenReceived;
                FirebaseMessaging.MessageReceived += OnMessageReceived;
                FirebaseMessaging.TokenRegistrationOnInitEnabled = true;

                // Get current token immediately
                string token = await FirebaseMessaging.GetTokenAsync();
                Debug.Log("FCM Token: " + token);
            }
            else
            {
                Debug.LogError("Cannot resolve Firebase dependencies: " + dependencyStatus);
            }
        }

        private void OnTokenReceived(object sender, TokenReceivedEventArgs token)
        {
            Debug.Log("Received FCM Token: " + token.Token);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Debug.Log("Received a new message from: " + e.Message.From);
        }
    }
}
