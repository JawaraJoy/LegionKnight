using UnityEngine;
using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections;
using UnityEngine.Events;

namespace LegionKnight
{
    public class InAppUpdate : MonoBehaviour
    {
        private AppUpdateManager m_AppUpdateManager;

        [SerializeField]
        private UnityEvent m_OnCheckForUpdate;
        [SerializeField]
        private UnityEvent<AppUpdateInfo> m_OnUpdateInfo;
        [SerializeField]
        private UnityEvent<AppUpdateInfo> m_OnUpdateAvailable;
        [SerializeField]
        private UnityEvent m_OnDownloaded;
        [SerializeField]
        private UnityEvent m_OnFailed;
        [SerializeField]
        private UnityEvent m_OnCanceled;

        private AppUpdateInfo m_AppUpdateInfo;
        private AppUpdateOptions m_AppUpdateOptions;

        public void CheckUpdate()
        {
            StartCoroutine(CheckingForUpdate());
        }

        private IEnumerator CheckingForUpdate()
        {
            m_OnCheckForUpdate?.Invoke();
#if UNITY_ANDROID && !UNITY_EDITOR
            var appUpdateManager = new AppUpdateManager();
            // run your real update check here
#else
            Debug.Log("In-App Update check skipped (not running on Android).");
#endif
            var appUpdateInfoAsync = m_AppUpdateManager.GetAppUpdateInfo();
            yield return appUpdateInfoAsync;
            if (appUpdateInfoAsync.IsSuccessful)
            {
                var appUpdateResult = appUpdateInfoAsync.GetResult();
                var appUpdateOption = AppUpdateOptions.ImmediateAppUpdateOptions();
                m_AppUpdateInfo = appUpdateResult;
                m_AppUpdateOptions = appUpdateOption;
                if (appUpdateResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
                {
                    m_OnUpdateAvailable?.Invoke(appUpdateResult);
                    //yield return StartCoroutine(StartingUpdate(appUpdateResult, appUpdateOption));
                }
                else
                {
                    m_OnDownloaded?.Invoke();
                }
            }
        }

        public void StartUpdate()
        {
            StartCoroutine(StartingUpdate(m_AppUpdateInfo, m_AppUpdateOptions));
        }

        private IEnumerator StartingUpdate(AppUpdateInfo appUpdateInfo, AppUpdateOptions appUpdateOptions)
        {
            var startUpdateRequest = m_AppUpdateManager.StartUpdate(appUpdateInfo, appUpdateOptions);
            m_OnUpdateInfo?.Invoke(appUpdateInfo);
            yield return startUpdateRequest;
            switch (startUpdateRequest.Status)
            {
                case AppUpdateStatus.Unknown:
                    break;
                case AppUpdateStatus.Pending:
                    break;
                case AppUpdateStatus.Downloading:
                    break;
                case AppUpdateStatus.Downloaded:
                    m_OnDownloaded?.Invoke();
                    break;
                case AppUpdateStatus.Installing:
                    break;
                case AppUpdateStatus.Installed:
                    break;
                case AppUpdateStatus.Failed:
                    m_OnFailed.Invoke();
                    break;
                case AppUpdateStatus.Canceled:
                    m_OnCanceled?.Invoke();
                    break;
                default:
                    Debug.LogError($"[InAppUpdate] {startUpdateRequest.Status} is out of context");
                    break;
            }
        }
    }
}
