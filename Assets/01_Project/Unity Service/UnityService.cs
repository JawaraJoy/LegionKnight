using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class UnityService : Singleton<UnityService>
    {
        [SerializeField]
        private string m_EnvironmentName = "production"; // Set your environment name here

        [SerializeField]
        private UnityEvent m_OnInitialized = new();
        [SerializeField]
        private UnityEvent<string> m_OnInitializationFailed = new();

        private bool m_IsInitialized = false;
        public bool IsInitialized => m_IsInitialized;
        async void Start()
        {
            try
            {
                var options = new InitializationOptions().SetEnvironmentName(m_EnvironmentName);

                await UnityServices.InitializeAsync(options);
                // Notify that Unity Services have been initialized successfully.
                m_OnInitialized.Invoke();
                m_IsInitialized = true;
                Debug.Log("Unity Services initialized successfully.");
            }
            catch (Exception exception)
            {
                // An error occurred during services initialization.
                string errorMessage = $"Failed to initialize Unity Services: {exception.Message}";
                Debug.LogError($"Failed to initialize Unity Services: {exception.Message}");
                // Notify that initialization failed.
                m_OnInitializationFailed.Invoke(errorMessage);
            }
        }
    }
}
