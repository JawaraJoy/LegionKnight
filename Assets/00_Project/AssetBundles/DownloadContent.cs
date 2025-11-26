using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public class DownloadContent : MonoBehaviour
    {
        [Header("Select your Addressable Label")]
        [SerializeField] private AssetLabelReference m_LabelToLoad;

        [Header("Events")]
        [SerializeField] private UnityEvent m_OnInit;
        [SerializeField] private UnityEvent<float> m_OnDownloadProgress;
        [SerializeField] private UnityEvent<long> m_OnDownloadSizeFound; // bytes
        [SerializeField] private UnityEvent m_OnDownloadComplete;
        [SerializeField] private UnityEvent m_OnContinue;
        [SerializeField] private UnityEvent m_OnDownloadCanceled;
        [SerializeField] private UnityEvent<string> m_OnLogMessage; // optional UI log output

        private bool? m_UserConfirmed = null; // null = undecided, true = confirmed, false = canceled
        private static bool s_Initialized = false; // cache initialization flag

        public void Init()
        {
            StartCoroutine(Initing());
        }

        public void Continue()
        {
            m_OnContinue?.Invoke();
        }

        private IEnumerator Initing()
        {
            if (m_LabelToLoad == null || string.IsNullOrEmpty(m_LabelToLoad.labelString))
            {
                LogError("Label not assigned. Aborting.");
                yield break;
            }

            m_OnInit?.Invoke();
            Log("Starting Addressables content check...");

            try
            {
                yield return InitializeAddressables();
                yield return CheckAndUpdateCatalog();

                bool proceed = false;
                yield return EnsureDependencies(result => proceed = result);

                if (proceed)
                {
                    yield return LoadAssetsFromLabel();
                }
                else
                {
                    Log("Download canceled or failed. Skipping load.");
                }
            }
            finally
            {
                m_OnDownloadComplete?.Invoke();
            }
        }

        private IEnumerator InitializeAddressables()
        {
            if (s_Initialized)
            {
                Log("Addressables already initialized. Skipping re-init.");
                yield break;
            }

            Log("Initializing Addressables...");
            var initHandle = Addressables.InitializeAsync();
            yield return initHandle;

            if (initHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Log("Addressables initialized successfully.");
                s_Initialized = true;
            }
            else
            {
                LogError("Failed to initialize Addressables!");
            }

            SafeRelease(initHandle);
        }

        private IEnumerator CheckAndUpdateCatalog()
        {
            Log("Checking for content updates...");
            var checkHandle = Addressables.CheckForCatalogUpdates(false);
            yield return checkHandle;

            if (checkHandle.Status == AsyncOperationStatus.Succeeded && checkHandle.Result.Count > 0)
            {
                Log($"Catalog update found ({checkHandle.Result.Count}). Updating...");
                var updateHandle = Addressables.UpdateCatalogs(checkHandle.Result);
                yield return updateHandle;

                if (updateHandle.Status == AsyncOperationStatus.Succeeded)
                    Log("Catalog updated successfully.");
                else
                    LogError("Catalog update failed!");

                SafeRelease(updateHandle);
            }
            else if (checkHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Log("No catalog updates found.");
            }
            else
            {
                LogError("Failed to check for catalog updates!");
            }

            SafeRelease(checkHandle);
        }

        private IEnumerator EnsureDependencies(System.Action<bool> callback)
        {
            Log($"Checking local cache for label: {m_LabelToLoad.labelString}");

            var sizeHandle = Addressables.GetDownloadSizeAsync(m_LabelToLoad);
            yield return sizeHandle;

            if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
            {
                LogError("Failed to check download size.");
                SafeRelease(sizeHandle);
                callback?.Invoke(false);
                yield break;
            }

            long downloadSize = sizeHandle.Result;
            SafeRelease(sizeHandle);

            if (downloadSize <= 0)
            {
                Log("All assets already cached. Skipping download.");
                m_OnDownloadProgress?.Invoke(1f);
                callback?.Invoke(true);
                yield break;
            }

            m_OnDownloadSizeFound?.Invoke(downloadSize);
            Log($"New content available! Size: {downloadSize / (1024f * 1024f):F2} MB");

            // Wait for player confirmation
            m_UserConfirmed = null;
            yield return new WaitUntil(() => m_UserConfirmed.HasValue);

            if (m_UserConfirmed == false)
            {
                Log("Download canceled by user. Quitting game...");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                callback?.Invoke(false);
                yield break;
            }

            Log("Starting content download...");
            var downloadHandle = Addressables.DownloadDependenciesAsync(m_LabelToLoad, true);

            while (!downloadHandle.IsDone)
            {
                m_OnDownloadProgress?.Invoke(downloadHandle.PercentComplete);
                yield return null;
            }

            if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Log("Download completed successfully!");
                m_OnDownloadProgress?.Invoke(1f);
                callback?.Invoke(true);
            }
            else
            {
                LogError("Download failed!");
                callback?.Invoke(false);
            }

            SafeRelease(downloadHandle);
        }

        private IEnumerator LoadAssetsFromLabel()
        {
            Log($"Loading assets with label: {m_LabelToLoad.labelString}");

            var loadHandle = Addressables.LoadAssetsAsync<GameObject>(m_LabelToLoad, obj =>
            {
                Instantiate(obj);
            });

            yield return loadHandle;

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
                Log($"Loaded {loadHandle.Result.Count} assets (from cache or cloud).");
            else
                LogError("Failed to load assets!");

            SafeRelease(loadHandle);
        }

        // Called by UI when player confirms
        public void ConfirmDownload() => m_UserConfirmed = true;

        // Called by UI when player cancels
        public void CancelDownload() => m_UserConfirmed = false;

        // Utility: Safe handle release
        private void SafeRelease(AsyncOperationHandle handle)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }

        // Logging helpers
        private void Log(string msg)
        {
            Debug.Log($"[DownloadContent] {msg}");
            m_OnLogMessage?.Invoke(msg);
        }

        private void LogError(string msg)
        {
            Debug.LogError($"[DownloadContent] {msg}");
            m_OnLogMessage?.Invoke($"ERROR: {msg}");
        }
    }

    public partial class UnityService
    {
        [SerializeField]
        private DownloadContent m_DownloadContent;
        public DownloadContent GetDownloadContent()
        {
            return m_DownloadContent;
        }
    }
}
