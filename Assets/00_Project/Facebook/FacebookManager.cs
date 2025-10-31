using Facebook.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class FacebookManager : Singleton<FacebookManager>
    {
        [SerializeField]
        private UnityEvent m_OnInitialized;

        private int m_LoginRetryCount = 0;

        private readonly string m_LoginKey = "fblogin";

        public void Init()
        {
            InitInternal();
        }
        private void InitInternal()
        {
            if (FB.IsInitialized)
            {
                OnInitialized();
            }
            else
            {
                //Handle FB.Init
                FB.Init(OnInitialized);
            }
        }

        private void OnInitialized()
        {
            FB.ActivateApp();
            m_OnInitialized?.Invoke();
            bool hasLoginToken = UnityService.Instance.HasData(m_LoginKey);
            if (hasLoginToken)
            {
                int loginCount = UnityService.Instance.GetData<int>(m_LoginKey);
                m_LoginRetryCount = loginCount + 1;
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"time", DateTime.UtcNow}
            };
            LoginEvent(m_LoginRetryCount, parameters);
        }
        void OnApplicationPause(bool pauseStatus)
        {
            // Check the pauseStatus to see if we are in the foreground
            // or background
            if (!pauseStatus)
            {
                InitInternal();
            }
        }

        private void LogEventInternal(string eventName, float valueToSum, Dictionary<string, object> parameters)
        {
            if (FB.IsInitialized)
            {
                FB.LogAppEvent(
                    eventName,
                    valueToSum,
                    parameters
                );
            }
        }
        
        private void LoginEvent(float valueToSum, Dictionary<string, object> parameters)
        {
            LogEventInternal(FacebookEventName.Login, valueToSum, parameters);
            UnityService.Instance.SaveData(m_LoginKey, m_LoginRetryCount);
        }
    }

    public static class FacebookEventName
    {
        public const string Login = "login";
    }
}
