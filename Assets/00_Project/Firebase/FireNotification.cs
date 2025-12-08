using Firebase.Messaging;
using UnityEngine;

namespace LegionKnight
{
    public class FireNotification : MonoBehaviour
    {
        private void Start()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith
                (
                    task =>
                    {
                        var dependencyStatus = task.Result;
                        if (dependencyStatus == Firebase.DependencyStatus.Available)
                        {
                            Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                        }
                        else
                        {
                            Debug.LogError($"{System.String.Format($"Cant Resolve all Firebase Dependecies: {0}", dependencyStatus)}");
                        }
                    }
                );
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
        }

        public void OnTokenReceived(object sender, TokenReceivedEventArgs token)
        {
            Debug.Log("Received Registration token:" + token.Token);
        }
        public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Debug.Log("Received a new message from:" + e.Message.From);
        }
    }
}
