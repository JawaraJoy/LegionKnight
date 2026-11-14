using Rush;
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
        private InitCondition m_InitCondition = InitCondition.Editor;
        [SerializeField]
        private DailyCheckIn m_DailyCheckIn;
        [SerializeField]
        private UnityEvent m_OnInitialized = new();
        [SerializeField]
        private UnityEvent<string> m_OnInitializationFailed = new();

        private bool m_IsInitialized = false;
        public DailyCheckIn DailyCheckIn => m_DailyCheckIn;
        public bool IsInitialized => m_IsInitialized;
        internal string EnvironmentName => m_EnvironmentName;
        private bool m_Initializing;
        private void Start() => InitializeServices();

        private async void InitializeServices()
        {
            if (m_IsInitialized || m_Initializing) return;
            m_Initializing = true;
            try
            {
                var options = new InitializationOptions().SetEnvironmentName(m_EnvironmentName);

                await UnityServices.InitializeAsync(options);
                // Notify that Unity Services have been initialized successfully.
                m_OnInitialized.Invoke();

                switch(m_InitCondition)
                {
                    case InitCondition.Editor:
                        m_AuthenticationManager.SignInAnonymously();
                        break;
                    case InitCondition.Build:
                        m_InAppUpdate.CheckUpdate();
                        break;
                    default:
                        Debug.LogWarning("Fail to init");
                        break;
                }
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
#if UNITY_ANDROID && !UNITY_EDITOR
                var panel = CanvasManager.Instance.GetPanel<AuthenticationPanel>();
                if (panel.TryGetComponent<AndroidPlayGamesSignIn>(out var login))
                    login.ShowInitializationFailure(InitializeServices);
#endif
            }
            finally { m_Initializing = false; }
        }
    }

    public enum InitCondition
    {
        Editor,
        Build,
    }
}
