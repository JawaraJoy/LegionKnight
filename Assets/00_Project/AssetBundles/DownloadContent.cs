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
        private ConfirmationTab Confirmation => _confirmation ??= Panel.GetBinding<ConfirmationTab>();

        private LoadingProgressTab _progress;
        private LoadingProgressTab Progress => _progress ??= Panel.GetBinding<LoadingProgressTab>();

        private CompleteTab _complete;
        private CompleteTab Complete => _complete ??= Panel.GetBinding<CompleteTab>();

        private FailTab _fail;
        private FailTab Fail => _fail ??= Panel.GetBinding<FailTab>();

        // ---------------------------------------------------------------
        // ENTRY
        // ---------------------------------------------------------------
        public void Init()
        {
            StartCoroutine(Process());
        }

        public void Confirm() => m_UserDecision = true;
        public void Cancel() => m_UserDecision = false;

        // ---------------------------------------------------------------
        // MAIN FLOW
        // ---------------------------------------------------------------
        private IEnumerator Process()
        {
            HideAllTabs();
            OnInit?.Invoke();
            Log("Starting download workflow...");

            // 1. Initialize
            yield return InitializeAddressables();
            if (!s_Initialized) yield break;

            // 2. Catalog Update
            yield return CheckCatalogUpdates();

            // 3. Size check + confirm + download
            bool continueProcess = false;
            yield return CheckDependencies(r => continueProcess = r);

            if (!continueProcess)
                yield break;

            // 4. SUCCESS
            yield return new WaitForEndOfFrame();
            ShowSuccess("Content Ready.");
        }

        // ---------------------------------------------------------------
        // INITIALIZATION
        // ---------------------------------------------------------------
        private IEnumerator InitializeAddressables()
        {
            if (s_Initialized) yield break;

            Log("Initializing Addressables...");
            var init = Addressables.InitializeAsync();
            yield return init;

            if (init.Status == AsyncOperationStatus.Succeeded)
            {
                s_Initialized = true;
                Log("Addressables initialized.");
            }
            else
            {
                ShowFail("Failed to initialize Addressables.");
            }

            SafeRelease(init);
        }

        // ---------------------------------------------------------------
        // CATALOG UPDATE
        // ---------------------------------------------------------------
        private IEnumerator CheckCatalogUpdates()
        {
            Log("Checking catalog updates...");

            var check = Addressables.CheckForCatalogUpdates();
            yield return check;

            if (check.Status != AsyncOperationStatus.Succeeded)
            {
                ShowFail("Catalog check failed.");
                SafeRelease(check);
                yield break;
            }

            if (check.Result.Count == 0)
            {
                Log("No catalog updates.");
                SafeRelease(check);
                yield break;
            }

            Log("Updating catalogs...");
            var update = Addressables.UpdateCatalogs(check.Result);
            yield return update;

            if (update.Status != AsyncOperationStatus.Succeeded)
                ShowFail("Catalog update failed.");

            SafeRelease(check);
            SafeRelease(update);
        }

        // ---------------------------------------------------------------
        // SIZE CHECK + CONFIRM + DOWNLOAD
        // ---------------------------------------------------------------
        private IEnumerator CheckDependencies(System.Action<bool> callback)
        {
            Log("Checking download size...");

            var sizeHandle = Addressables.GetDownloadSizeAsync(m_Label);
            yield return sizeHandle;

            if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
            {
                ShowFail("Failed to get size.");
                callback(false);
                yield break;
            }

            long bytes = sizeHandle.Result;
            SafeRelease(sizeHandle);

            if (bytes <= 0)
            {
                Log("All content already cached.");
                callback(true);
                yield break;
            }

            // Tell UI
            Panel.Show();
            OnSizeFound?.Invoke(bytes);
            Confirmation.ConfirDownload(bytes);
            OnShowConfirmation?.Invoke();

            // Wait confirm
            m_UserDecision = null;
            yield return new WaitUntil(() => m_UserDecision.HasValue);

            if (m_UserDecision == false)
            {
                ShowFail("User cancelled.");
                callback(false);
                yield break;
            }

            bool result = false;
            yield return DownloadAndLoad(bytes, r => result = r);

            callback(result);
        }

        // ---------------------------------------------------------------
        // DOWNLOAD + LOAD ASSET FIXED VERSION
        // ---------------------------------------------------------------
        private IEnumerator DownloadAndLoad(long size, System.Action<bool> callback)
        {
            Panel.Show();
            OnShowProgress?.Invoke();
            Progress.Show();

            Log("Downloading...");
            var download = Addressables.DownloadDependenciesAsync(m_Label, true);

            float stuckTimer = 0f;
            float lastPercent = 0f;

            while (!download.IsDone)
            {
                float p = download.PercentComplete;
                OnProgress?.Invoke(p);
                Progress.SetProgress(p);

                // Anti-stuck fix (0.99 freeze)
                if (p >= 0.985f)
                {
                    if (Mathf.Approximately(p, lastPercent))
                        stuckTimer += Time.deltaTime;
                    else
                        stuckTimer = 0f;

                    lastPercent = p;

                    if (stuckTimer >= 3f)
                    {
                        Debug.LogWarning("[DownloadContent] STUCK → forcing completion.");
                        break;
                    }
                }

                yield return null;
            }

            bool downloadSuccess = download.Status == AsyncOperationStatus.Succeeded;
            SafeRelease(download);

            // FINAL SUCCESS VALIDATION
            long remaining = Addressables.GetDownloadSizeAsync(m_Label).Result;

            // Correct definition:
            bool finalSuccess = downloadSuccess || remaining == 0;

            Progress.Hide();

            if (!finalSuccess)
            {
                ShowFail("Download failed.");
                callback(false);
                yield break;
            }

            // LOAD ASSETS (to verify bundle is valid)
            Log("Loading assets...");
            var load = Addressables.LoadAssetsAsync<Object>(m_Label, _ => { });
            yield return load;

            if (load.Status != AsyncOperationStatus.Succeeded)
            {
                ShowFail("Asset loading failed.");
                SafeRelease(load);
                callback(false);
                yield break;
            }

            SafeRelease(load);
            callback(true);
        }

        // ---------------------------------------------------------------
        // UI HELPERS
        // ---------------------------------------------------------------
        private void HideAllTabs()
        {
            Confirmation?.Hide();
            Progress?.Hide();
            Complete?.Hide();
            Fail?.Hide();
        }

        private void ShowSuccess(string msg)
        {
            HideAllTabs();
            Panel.Show();
            Complete.Show();
            OnShowSuccess?.Invoke();
            Log(msg);
        }

        private void ShowFail(string msg)
        {
            HideAllTabs();
            Panel.Show();
            Fail.Show();
            OnShowFail?.Invoke();
            LogError(msg);
        }

        // ---------------------------------------------------------------
        // UTILS
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

