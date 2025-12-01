using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public partial class DownloadContent : MonoBehaviour
    {
        [Header("Select Label")]
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

        // UI panels
        private DownloadPanel Panel => GameManager.Instance.GetPanel<DownloadPanel>();
        private ConfirmationTab ConfirmTab => Panel.GetBinding<ConfirmationTab>();
        private LoadingProgressTab ProgressTab => Panel.GetBinding<LoadingProgressTab>();
        private CompleteTab CompleteTab => Panel.GetBinding<CompleteTab>();
        private FailTab FailTab => Panel.GetBinding<FailTab>();

        // ---------------------------------------------------------
        public void Init()
        {
            StartCoroutine(Process());
        }

        public void Confirm() => m_UserDecision = true;
        public void Cancel() => m_UserDecision = false;

        // ---------------------------------------------------------
        private IEnumerator Process()
        {
            HideAllTabs();
            OnInit?.Invoke();

            // 1. Initialize addressables
            yield return InitializeAddressables();
            if (!s_Initialized)
                yield break;

            // 2. Catalog updates
            yield return CheckCatalogUpdates();

            // 3. Size check + confirmation + download
            bool ok = false;
            yield return CheckDependencies(r => ok = r);

            if (!ok)
                yield break;

            // 4. Load assets to ensure Unity marks bundle complete
            yield return LoadAtLeastOneAsset();

            ShowSuccess("Content ready.");
        }

        // ---------------------------------------------------------
        private IEnumerator InitializeAddressables()
        {
            if (s_Initialized)
                yield break;

            var init = Addressables.InitializeAsync();
            yield return init;

            if (init.Status == AsyncOperationStatus.Succeeded)
                s_Initialized = true;
            else
                ShowFail("Addressables initialization failed.");

            SafeRelease(init);
        }

        // ---------------------------------------------------------
        private IEnumerator CheckCatalogUpdates()
        {
            var check = Addressables.CheckForCatalogUpdates(false);
            yield return check;

            if (check.Status != AsyncOperationStatus.Succeeded)
            {
                ShowFail("Catalog check failed.");
                SafeRelease(check);
                yield break;
            }

            if (check.Result.Count > 0)
            {
                var update = Addressables.UpdateCatalogs(check.Result);
                yield return update;

                if (update.Status != AsyncOperationStatus.Succeeded)
                {
                    ShowFail("Catalog update failed.");
                    SafeRelease(check);
                    SafeRelease(update);
                    yield break;
                }

                SafeRelease(update);
            }

            SafeRelease(check);
        }

        // ---------------------------------------------------------
        private IEnumerator CheckDependencies(System.Action<bool> callback)
        {
            var sizeHandle = Addressables.GetDownloadSizeAsync(m_Label);
            yield return sizeHandle;

            if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
            {
                ShowFail("Failed to check download size.");
                callback(false);
                yield break;
            }

            long bytes = sizeHandle.Result;
            SafeRelease(sizeHandle);

            if (bytes <= 0)
            {
                callback(true);
                yield break;
            }

            // Show confirmation
            Panel.Show();
            OnSizeFound?.Invoke(bytes);
            ConfirmTab.ConfirDownload(bytes);
            OnShowConfirmation?.Invoke();

            m_UserDecision = null;
            yield return new WaitUntil(() => m_UserDecision.HasValue);

            if (m_UserDecision == false)
            {
                ShowFail("User canceled download.");
                callback(false);
                yield break;
            }

            // Download
            bool ok = false;
            yield return DownloadBundles(bytes, r => ok = r);

            callback(ok);
        }

        // ---------------------------------------------------------
        private IEnumerator DownloadBundles(long size, System.Action<bool> callback)
        {
            Panel.Show();
            OnShowProgress?.Invoke();
            ProgressTab.Show();

            var download = Addressables.DownloadDependenciesAsync(m_Label, true);

            float freezeTimer = 0f;
            float lastP = 0f;

            while (!download.IsDone)
            {
                float p = download.PercentComplete;
                OnProgress?.Invoke(p);
                ProgressTab.SetProgress(p);

                // freeze detection
                if (Mathf.Approximately(p, lastP))
                    freezeTimer += Time.deltaTime;
                else
                    freezeTimer = 0f;

                lastP = p;

                if (freezeTimer > 3f)
                {
                    Debug.LogWarning("Force finishing — download stuck.");
                    break;
                }

                yield return null;
            }

            bool success = download.Status == AsyncOperationStatus.Succeeded;
            SafeRelease(download);

            ProgressTab.Hide();

            if (!success)
            {
                ShowFail("Download Failed.");
                callback(false);
                yield break;
            }

            callback(true);
        }

        // ---------------------------------------------------------
        private IEnumerator LoadAtLeastOneAsset()
        {
            var load = Addressables.LoadAssetsAsync<Object>(m_Label, null);
            yield return load;
            SafeRelease(load);
        }

        // ---------------------------------------------------------
        private void HideAllTabs()
        {
            ConfirmTab.Hide();
            ProgressTab.Hide();
            CompleteTab.Hide();
            FailTab.Hide();
        }

        private void ShowSuccess(string msg)
        {
            HideAllTabs();
            Panel.Show();
            CompleteTab.Show();
            OnShowSuccess?.Invoke();
        }

        private void ShowFail(string msg)
        {
            HideAllTabs();
            Panel.Show();
            FailTab.Show();
            OnShowFail?.Invoke();
        }

        // ---------------------------------------------------------
        private void SafeRelease(AsyncOperationHandle handle)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
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
