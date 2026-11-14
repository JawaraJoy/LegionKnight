using System;
using System.Threading.Tasks;
using LegionKnight;
using TMPro;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class AndroidPlayGamesSignIn : MonoBehaviour
    {
        private const string c_Owner = "auth.playgames.owner";
        [SerializeField] private GameObject m_Layout;
        [SerializeField] private GameObject[] m_Legacy;
        [SerializeField] private TMP_Text m_Status;
        [SerializeField] private Button m_Retry;
        private bool m_InFlight, m_Complete;
        private string m_Environment;
        private string OwnerKey => c_Owner + "." + m_Environment;
        private Action<string> m_OnReady;
        private Action m_RetryInitialization;

        private void Awake()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            foreach (var view in m_Legacy) view.SetActive(false);
            m_Layout.SetActive(true);
            m_Retry.onClick.AddListener(RetryInternal);
            DisplayInternal(false);
#endif
        }

        public void Begin(string environment, Action<string> onReady)
        {
            m_Environment = environment;
            m_OnReady = onReady;
            m_RetryInitialization = null;
            RunInternal(false);
        }

        public void ShowInitializationFailure(Action retryInitialization)
        {
            m_RetryInitialization = retryInitialization;
            DisplayInternal(true);
        }

        private void RetryInternal()
        {
            if (m_InFlight || m_Complete) return;
            DisplayInternal(false);
            if (m_RetryInitialization != null) m_RetryInitialization();
            else RunInternal(true);
        }

        private void DisplayInternal(bool failed)
        {
            m_Status.gameObject.SetActive(!failed);
            m_Status.text = "Signing in…";
            m_Retry.gameObject.SetActive(failed);
        }

        private async void RunInternal(bool interactive)
        {
            if (m_InFlight || m_Complete) return;
            m_InFlight = true;
            DisplayInternal(false);
            string owner;
            try
            {
                var service = UnityService.Instance;
                var account = new PlayGamesUnityAccount(m_Environment);
                var platform = new PlayGamesAndroidClient();
                owner = await new PlayGamesAccountFlow().AuthenticateAsync(account, PlayerPrefs.GetString(OwnerKey),
                    service.HasData("playerName"), () => platform.RequestCodeAsync(interactive), RememberOwner);
            }
            catch (Exception exception)
            {
                // OAuth codes and service response bodies must never reach logs or analytics.
                Debug.LogWarning("Play Games sign-in failed (" + exception.GetType().Name + ").");
                if (this) DisplayInternal(true);
                return;
            }
            finally { m_InFlight = false; }
            if (!this) return;
            m_Complete = true;
            m_OnReady(owner);
        }

        private void RememberOwner(string id)
        {
            if (PlayerPrefs.GetString(OwnerKey) == id) return;
            PlayerPrefs.SetString(OwnerKey, id);
            PlayerPrefs.Save();
        }
    }
}
