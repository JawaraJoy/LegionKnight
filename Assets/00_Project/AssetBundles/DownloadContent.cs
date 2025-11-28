using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public partial class DownloadContent : MonoBehaviour
    {
        [Header("Select your Addressable Label")]
        [SerializeField] private AssetLabelReference m_Label;

        [Header("UI Events")]
        [SerializeField] private UnityEvent OnInit;
        [SerializeField] private UnityEvent OnShowConfirmation;
        [SerializeField] private UnityEvent OnShowProgress;
        [SerializeField] private UnityEvent OnShowSuccess;
        [SerializeField] private UnityEvent OnContinueAfterSuccess;
        [SerializeField] private UnityEvent OnShowFail;

        [SerializeField] private UnityEvent<long> OnSizeFound;
        [SerializeField] private UnityEvent<float> OnProgress;
        [SerializeField] private UnityEvent<string> OnLog;

        private bool? m_UserDecision = null;
        private static bool s_Initialized;

        public UnityEvent OnContinueAfterSuccessPublic => OnContinueAfterSuccess;

        // --- UI Bindings ---
        private DownloadPanel _panel;
        private DownloadPanel Panel => _panel ??= GameManager.Instance.GetPanel<DownloadPanel>();

        private ConfirmationTab _confirmation;
        private ConfirmationTab Confirmation =>
            _confirmation ??= Panel.GetBinding<ConfirmationTab>();

        private LoadingProgressTab _progress;
        private LoadingProgressTab Progress =>
            _progress ??= Panel.GetBinding<LoadingProgressTab>();

        private CompleteTab _complete;
        private CompleteTab Complete =>
            _complete ??= Panel.GetBinding<CompleteTab>();

        private FailTab _fail;
        private FailTab Fail =>
            _fail ??= Panel.GetBinding<FailTab>();

        // ---------------------------------------------------------------
        // PUBLIC METHODS
        // ---------------------------------------------------------------
        public void Init() => StartCoroutine(Process());

        public void Confirm() => m_UserDecision = true;
        public void Cancel() => m_UserDecision = false;

        // ---------------------------------------------------------------
        // MAIN FLOW
        // ---------------------------------------------------------------
        private IEnumerator Process()
        {
            HideAllTabs();

            OnInit?.Invoke();
            Log("Started downloadable content check...");

            // 1. Addressables init
            yield return InitializeAddressables();
            if (!s_Initialized)
                yield break;

            // 2. Catalog check
            yield return CheckCatalogUpdates();

            // 3. Detect required download size
            bool canContinue = false;
            yield return CheckDependencies(result => canContinue = result);

            if (!canContinue)
                yield break;

            // 4. Load assets (optional, recommended)
            bool loadSuccess = false;
            yield return LoadAssets(result => loadSuccess = result);

            if (!loadSuccess)
            {
                ShowFail("Asset loading failed.");
                yield break;
            }

            // 5. Success
            ShowSuccess("All content is ready.");
        }

        // ---------------------------------------------------------------
        // INITIALIZATION
        // ---------------------------------------------------------------
        private IEnumerator InitializeAddressables()
        {
            if (s_Initialized)
                yield break;

            Log("Initializing Addressables...");
            var handle = Addressables.InitializeAsync();
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                s_Initialized = true;
                Log("Addressables initialized.");
            }
            else
            {
                ShowFail("Failed to initialize Addressables.");
            }

            SafeRelease(handle);
        }

        // ---------------------------------------------------------------
        // CATALOG UPDATE
        // ---------------------------------------------------------------
        private IEnumerator CheckCatalogUpdates()
        {
            Log("Checking catalog updates...");

            var check = Addressables.CheckForCatalogUpdates(false);
            yield return check;

            if (check.Status != AsyncOperationStatus.Succeeded)
            {
                ShowFail("Catalog check failed.");
                SafeRelease(check);
                yield break;
            }

            if (check.Result.Count == 0)
            {
                Log("No catalog updates found.");
                SafeRelease(check);
                yield break;
            }

            Log($"Catalog updates found: {check.Result.Count}. Updating...");
            var update = Addressables.UpdateCatalogs(check.Result);
            yield return update;

            if (update.Status == AsyncOperationStatus.Succeeded)
                Log("Catalog updated.");
            else
                ShowFail("Catalog update failed.");

            SafeRelease(check);
            SafeRelease(update);
        }

        // ---------------------------------------------------------------
        // DOWNLOAD SIZE CHECK + CONFIRMATION UI
        // ---------------------------------------------------------------
        private IEnumerator CheckDependencies(System.Action<bool> result)
        {
            Log("Checking download size...");

            var sizeHandle = Addressables.GetDownloadSizeAsync(m_Label);
            yield return sizeHandle;

            if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
            {
                ShowFail("Failed to read download size.");
                result(false);
                yield break;
            }

            long size = sizeHandle.Result;
            SafeRelease(sizeHandle);

            if (size <= 0)
            {
                Log("All content is already cached.");
                result(true);
                yield break;
            }

            // UI: Ask player
            Panel.Show();
            OnSizeFound?.Invoke(size);
            Confirmation.ConfirDownload(size);
            OnShowConfirmation?.Invoke();

            m_UserDecision = null;
            yield return new WaitUntil(() => m_UserDecision.HasValue);

            // If player cancels
            if (m_UserDecision == false)
            {
                ShowFail("User canceled download.");
                result(false);
                yield break;
            }

            // Player agreed → start download
            bool downloadSuccess = false;
            yield return DownloadContentAsync(size, res => downloadSuccess = res);

            result(downloadSuccess);
        }

        // ---------------------------------------------------------------
        // DOWNLOAD CONTENT WITH SMOOTH PROGRESS
        // ---------------------------------------------------------------
        private IEnumerator DownloadContentAsync(long size, System.Action<bool> callback)
        {
            Log($"Downloading {size / (1024f * 1024f):F2} MB...");

            Panel.Show();
            OnShowProgress?.Invoke();
            Progress.Show();

            var download = Addressables.DownloadDependenciesAsync(m_Label, true);

            float nextUpdate = 0f;

            while (!download.IsDone)
            {
                float p = download.PercentComplete;
                OnProgress?.Invoke(p);

                if (Time.time > nextUpdate)
                {
                    Progress.LogMessage($"Downloading... {(p * 100f):F1}%");
                    nextUpdate = Time.time + 0.15f;
                }

                yield return null;
            }

            bool success = download.Status == AsyncOperationStatus.Succeeded;

            SafeRelease(download);
            Progress.Hide();

            Log(success ? "Download completed." : "Download failed.");

            callback(success);
        }

        // ---------------------------------------------------------------
        // LOAD DOWNLOADED ASSETS (optional)
        // ---------------------------------------------------------------
        private IEnumerator LoadAssets(System.Action<bool> callback)
        {
            Log("Loading downloaded assets...");

            var locHandle = Addressables.LoadResourceLocationsAsync(m_Label);
            yield return locHandle;

            if (locHandle.Status != AsyncOperationStatus.Succeeded)
            {
                SafeRelease(locHandle);
                callback(false);
                yield break;
            }

            var locations = locHandle.Result;
            int total = locations.Count;
            int loaded = 0;

            Panel.Show();
            OnShowProgress?.Invoke();
            Progress.Show();

            foreach (var loc in locations)
            {
                var h = Addressables.LoadAssetAsync<Object>(loc);
                yield return h;

                if (h.Status != AsyncOperationStatus.Succeeded)
                {
                    LogError("Asset load failed.");
                    SafeRelease(h);
                    SafeRelease(locHandle);
                    Progress.Hide();
                    callback(false);
                    yield break;
                }

                loaded++;
                float p = loaded / (float)total;
                OnProgress?.Invoke(p);
                Progress.LogMessage($"Loading... {(p * 100f):F1}%");

                SafeRelease(h);
                yield return null;
            }

            SafeRelease(locHandle);
            Progress.Hide();
            callback(true);
        }

        // ---------------------------------------------------------------
        // UI HELPERS
        // ---------------------------------------------------------------
        private void HideAllTabs()
        {
            Confirmation.Hide();
            Progress.Hide();
            Complete.Hide();
            Fail.Hide();
        }

        private void ShowSuccess(string msg)
        {
            HideAllTabs();
            Panel.Show();
            Complete.Show();
            Log(msg);
            OnShowSuccess?.Invoke();
        }

        private void ShowFail(string msg)
        {
            HideAllTabs();
            Panel.Show();
            Fail.Show();
            LogError(msg);
            OnShowFail?.Invoke();
        }

        // ---------------------------------------------------------------
        // UTILITY
        // ---------------------------------------------------------------
        private void SafeRelease(AsyncOperationHandle handle)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }

        private void Log(string msg)
        {
            Debug.Log("[DownloadContent] " + msg);
            OnLog?.Invoke(msg);
        }

        private void LogError(string msg)
        {
            Debug.LogError("[DownloadContent ERROR] " + msg);
            OnLog?.Invoke("ERROR: " + msg);
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
