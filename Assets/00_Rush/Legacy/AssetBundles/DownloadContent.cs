using System.Collections;
using Rush.ContentDelivery;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace LegionKnight
{
    // Existing serialized event and label names are retained for the authored download panel.
    public partial class DownloadContent : MonoBehaviour
    {
        [SerializeField] private AssetLabelReference m_Label;
        [SerializeField] private UnityEvent OnInit;
        [SerializeField] private UnityEvent OnShowConfirmation;
        [SerializeField] private UnityEvent OnShowProgress;
        [SerializeField] private UnityEvent OnShowSuccess;
        [SerializeField] private UnityEvent OnContinueAfterSuccess;
        [SerializeField] private UnityEvent OnShowFail;
        [SerializeField] private UnityEvent<long> OnSizeFound;
        [SerializeField] private UnityEvent<float> OnProgress;
        [SerializeField] private UnityEvent<string> OnLog;
        private bool? m_UserDecision;
        private Coroutine m_Routine;
        private bool m_Running;
        private ContentDownloadSession m_Session;
        public UnityEvent OnContinueAfterSuccessPublic => OnContinueAfterSuccess;
        private DownloadPanel Panel => CanvasManager.Instance.GetPanel<DownloadPanel>();
        private ConfirmationTab ConfirmTab => Panel.GetBinding<ConfirmationTab>();
        private LoadingProgressTab ProgressTab => Panel.GetBinding<LoadingProgressTab>();
        private CompleteTab CompleteTab => Panel.GetBinding<CompleteTab>();
        private FailTab FailTab => Panel.GetBinding<FailTab>();

        public void Init() => BeginInternal();
        public void Confirm() => m_UserDecision = true;
        public void Cancel() => m_UserDecision = false;

        private void BeginInternal()
        {
            if (m_Running) return;
            m_Running = true;
            m_Session = new ContentDownloadSession();
            m_Routine = StartCoroutine(ProcessInternal());
        }

        private IEnumerator ProcessInternal()
        {
            try
            {
                HideAllTabsInternal();
                OnInitInvoke();
                yield return m_Session.Initialize();
                if (!m_Session.Succeeded) { ShowFailInternal(m_Session.Error); yield break; }
                // Content catalogs are pinned to each compatible app build. The bootstrap has
                // already prepared this version; do not replace a catalog while its scene is loaded.
                yield return m_Session.Inspect(m_Label);
                if (!m_Session.Succeeded) { ShowFailInternal(m_Session.Error); yield break; }
                if (m_Session.RequiredBytes > 0)
                {
                    m_UserDecision = null;
                    Panel.Show();
                    OnSizeFoundInvoke(m_Session.RequiredBytes);
                    ConfirmTab.ConfirDownload(m_Session.RequiredBytes);
                    OnShowConfirmationInvoke();
                    yield return new WaitUntil(() => m_UserDecision.HasValue);
                    if (m_UserDecision == false) { ShowFailInternal("Download canceled."); yield break; }
                    Panel.Show();
                    ProgressTab.Show();
                    OnShowProgressInvoke();
                    yield return m_Session.Download(m_Label, UpdateProgressInternal);
                    ProgressTab.Hide();
                    if (!m_Session.Succeeded) { ShowFailInternal(m_Session.Error); yield break; }
                }
                // A download is validated by the cache-size check. Do not eagerly load every asset.
                HideAllTabsInternal();
                Panel.Show();
                CompleteTab.Show();
                OnShowSuccessInvoke();
            }
            finally { m_Running = false; m_Routine = null; }
        }

        private void UpdateProgressInternal(float value)
        {
            ProgressTab.SetProgress(value);
            OnProgressInvoke(value);
        }
        private void HideAllTabsInternal()
        {
            ConfirmTab.Hide(); ProgressTab.Hide(); CompleteTab.Hide(); FailTab.Hide();
        }
        private void ShowFailInternal(string message)
        {
            HideAllTabsInternal(); Panel.Show(); FailTab.Show();
            OnLogInvoke(message); OnShowFailInvoke();
        }
        private void OnDisable()
        {
            if (m_Routine != null) StopCoroutine(m_Routine);
            m_Routine = null;
            m_Running = false;
        }
        private void OnInitInvoke() => OnInit?.Invoke();
        private void OnShowConfirmationInvoke() => OnShowConfirmation?.Invoke();
        private void OnShowProgressInvoke() => OnShowProgress?.Invoke();
        private void OnShowSuccessInvoke() => OnShowSuccess?.Invoke();
        private void OnShowFailInvoke() => OnShowFail?.Invoke();
        private void OnSizeFoundInvoke(long value) => OnSizeFound?.Invoke(value);
        private void OnProgressInvoke(float value) => OnProgress?.Invoke(value);
        private void OnLogInvoke(string value) => OnLog?.Invoke(value);
    }

    public partial class UnityService
    {
        [SerializeField] private DownloadContent m_DownloadContent;
        public DownloadContent GetDownloadContent() => m_DownloadContent;
    }
}
