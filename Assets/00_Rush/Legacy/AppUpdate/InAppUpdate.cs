using UnityEngine;
using Google.Play.AppUpdate;
using Google.Play.Common;
using System.Collections;
using UnityEngine.Events;

namespace LegionKnight
{
    public enum UpdateMode
    {
        Immediate,
        Flexible
    }

    public class InAppUpdate : MonoBehaviour
    {
        private AppUpdateManager m_AppUpdateManager;
        private AppUpdateInfo m_AppUpdateInfo;
        private AppUpdateOptions m_AppUpdateOptions;

        [Header("Settings")]
        [SerializeField] private UpdateMode m_UpdateMode = UpdateMode.Flexible;

        [Header("Events")]
        [SerializeField] private UnityEvent m_OnCheckForUpdate;
        [SerializeField] private UnityEvent<AppUpdateInfo> m_OnUpdateInfo;
        [SerializeField] private UnityEvent<AppUpdateInfo> m_OnUpdateAvailable;
        [SerializeField] private UnityEvent m_OnUpToDate;
        [SerializeField] private UnityEvent m_OnDownloaded;
        [SerializeField] private UnityEvent m_OnFailed;
        [SerializeField] private UnityEvent m_OnCanceled;
        [SerializeField] private UnityEvent<float> m_OnProgressChanged; // 👈 Progress (0f - 1f)

        /// <summary>
        /// Call this to begin checking for updates.
        /// </summary>
        public void CheckUpdate()
        {
            StartCoroutine(CheckingForUpdate());
        }

        private IEnumerator CheckingForUpdate()
        {
            m_OnCheckForUpdate?.Invoke();

#if UNITY_ANDROID && !UNITY_EDITOR
            m_AppUpdateManager = new AppUpdateManager();
            var appUpdateInfoAsync = m_AppUpdateManager.GetAppUpdateInfo();
            yield return appUpdateInfoAsync;

            if (appUpdateInfoAsync.IsSuccessful)
            {
                var appUpdateResult = appUpdateInfoAsync.GetResult();
                m_AppUpdateInfo = appUpdateResult;

                // Pick update option based on Inspector setting
                m_AppUpdateOptions = (m_UpdateMode == UpdateMode.Immediate)
                    ? AppUpdateOptions.ImmediateAppUpdateOptions()
                    : AppUpdateOptions.FlexibleAppUpdateOptions();

                if (appUpdateResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
                {
                    Debug.Log($"[InAppUpdate] Update available. Version: {appUpdateResult.AvailableVersionCode}");
                    m_OnUpdateAvailable?.Invoke(appUpdateResult);
                }
                else
                {
                    Debug.Log("[InAppUpdate] App is already up to date.");
                    m_OnUpToDate?.Invoke();
                }
            }
            else
            {
                Debug.LogError("[InAppUpdate] Failed to check update info.");
                m_OnFailed?.Invoke();
            }
#else
            Debug.Log("[InAppUpdate] In-App Update check skipped (not running on Android).");
            yield break;
#endif
        }

        /// <summary>
        /// Starts the update flow after CheckUpdate detects an available update.
        /// </summary>
        public void StartUpdate()
        {
            if (m_AppUpdateInfo == null || m_AppUpdateOptions == null)
            {
                Debug.LogError("[InAppUpdate] Cannot start update: no update info available. Call CheckUpdate() first.");
                return;
            }

            StartCoroutine(StartingUpdate(m_AppUpdateInfo, m_AppUpdateOptions));
        }

        private IEnumerator StartingUpdate(AppUpdateInfo appUpdateInfo, AppUpdateOptions appUpdateOptions)
        {
            var startUpdateRequest = m_AppUpdateManager.StartUpdate(appUpdateInfo, appUpdateOptions);
            m_OnUpdateInfo?.Invoke(appUpdateInfo);

            // Loop until request finishes
            while (!startUpdateRequest.IsDone)
            {
                // Flexible updates support progress tracking
                if (m_UpdateMode == UpdateMode.Flexible)
                {
                    float progress = 0f;

                    if (startUpdateRequest.BytesDownloaded > 0 && startUpdateRequest.TotalBytesToDownload > 0)
                    {
                        progress = (float)startUpdateRequest.BytesDownloaded / startUpdateRequest.TotalBytesToDownload;
                        m_OnProgressChanged?.Invoke(progress);
                    }
                }
                yield return null;
            }

            // After completion
            switch (startUpdateRequest.Status)
            {
                case AppUpdateStatus.Downloaded:
                    Debug.Log("[InAppUpdate] Update downloaded.");
                    m_OnDownloaded?.Invoke();

                    if (m_UpdateMode == UpdateMode.Flexible)
                    {
                        // For Flexible updates, you must call CompleteUpdate to install
                        m_AppUpdateManager.CompleteUpdate();
                    }
                    break;

                case AppUpdateStatus.Failed:
                    Debug.LogError("[InAppUpdate] Update failed.");
                    m_OnFailed?.Invoke();
                    break;

                case AppUpdateStatus.Canceled:
                    Debug.LogWarning("[InAppUpdate] Update canceled by user.");
                    m_OnCanceled?.Invoke();
                    break;

                default:
                    Debug.Log($"[InAppUpdate] Status: {startUpdateRequest.Status}");
                    break;
            }
        }
    }
}
