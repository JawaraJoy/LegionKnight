using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Rush.ContentDelivery
{
    public sealed class CcdBootstrap : MonoBehaviour
    {
        [Serializable]
        private sealed class DownloadViewField
        {
            [SerializeField] private GameObject m_Content;
            [SerializeField] private Text m_Status;
            [SerializeField] private Text m_Detail;
            [SerializeField] private Slider m_Progress;
            [SerializeField] private Button m_Continue;
            [SerializeField] private Text m_ContinueLabel;

            [SerializeField] private RectTransform m_SafeArea;
            [SerializeField] private Text m_MetricLabel;
            [SerializeField] private Text m_MetricValue;
            [SerializeField] private Text m_Percent;
            [SerializeField] private Image m_Emblem;
            [SerializeField] private Image m_ActionIcon;
            [SerializeField] private Sprite m_DownloadIcon;
            [SerializeField] private Sprite m_CloudIcon;
            [SerializeField] private Sprite m_WarningIcon;
            [SerializeField] private Sprite m_RetryIcon;
            [SerializeField] private AudioSource m_ConfirmationSound;
            private int m_LastPercent = -1;
            private long m_LastAmount = -1;
            private long m_LastTotal = -1;

            internal void FitSafeArea(Rect area, int width, int height)
            {
                if (area.width <= 0 || area.height <= 0 || width <= 0 || height <= 0) return;
                m_SafeArea.anchorMin = new Vector2(area.xMin / width, area.yMin / height);
                m_SafeArea.anchorMax = new Vector2(area.xMax / width, area.yMax / height);
            }

            internal void Bind(Action clicked) => m_Continue.onClick.AddListener(() => clicked());

            internal void ShowWaiting(string title, string detail) =>
                DisplayInternal(title, detail, null, null, m_CloudIcon, null, false);

            internal void ShowConfirmation(long bytes) => DisplayInternal("Download Content",
                "Download the game files to start playing.\nWi-Fi is recommended.",
                "DOWNLOAD SIZE", FormatAmount(bytes), m_DownloadIcon, "Download", false, confirmSize: true);

            internal void ShowDownloading(long bytes)
            {
                DisplayInternal("Downloading", "Keep AetherJump open while\nthe game files download.",
                    "DOWNLOADED", null, m_CloudIcon, null, true);
                m_LastAmount = m_LastTotal = -1;
                m_LastPercent = -1;
                SetProgress(0, bytes, 0f);
            }

            internal void ShowFailure(string reason) => DisplayInternal("Download Failed", reason,
                "COMPLETED DOWNLOADS", "Saved on this device", m_WarningIcon, "Retry", false);

            private void DisplayInternal(string title, string detail, string metric, string amount,
                Sprite emblem, string action, bool downloading, bool confirmSize = false)
            {
                if (m_ConfirmationSound != null) m_ConfirmationSound.gameObject.SetActive(confirmSize);
                m_Status.text = title;
                m_Detail.text = detail;
                m_MetricLabel.text = metric ?? string.Empty;
                m_MetricValue.text = amount ?? string.Empty;
                m_Emblem.sprite = emblem;
                m_ContinueLabel.text = action ?? string.Empty;
                m_ActionIcon.sprite = emblem == m_WarningIcon ? m_RetryIcon : m_DownloadIcon;
                m_Continue.gameObject.SetActive(action != null);
                m_Progress.gameObject.SetActive(downloading);
                m_Percent.gameObject.SetActive(downloading);
            }

            internal void SetProgress(long downloaded, long total, float progress)
            {
                m_Progress.SetValueWithoutNotify(progress);
                int percent = Mathf.FloorToInt(Mathf.Clamp01(progress) * 100f);
                if (m_LastPercent != percent)
                {
                    m_Percent.text = percent.ToString() + "%";
                    m_LastPercent = percent;
                }
                if (downloaded == m_LastAmount && total == m_LastTotal) return;
                m_MetricValue.text = total < c_BytesPerMb
                    ? FormatAmount(downloaded) + " / " + FormatAmount(total)
                    : FormattableString.Invariant($"{downloaded / c_BytesPerMb:F1} / {total / c_BytesPerMb:F1} MB");
                m_LastAmount = downloaded;
                m_LastTotal = total;
            }

            private static string FormatAmount(long bytes)
            {
                if (bytes >= c_BytesPerMb) return FormattableString.Invariant($"{bytes / c_BytesPerMb:F1} MB");
                if (bytes >= 1000) return FormattableString.Invariant($"{bytes / 1000d:F1} KB");
                return bytes + " B";
            }
            internal void Hide() => m_Content.SetActive(false);
        }

        private const double c_BytesPerMb = 1000000d;
        [SerializeField] private string m_GameSceneKey = "Aether_Home (Used)";
        [SerializeField] private string m_DownloadLabel = "Downloadable";
        [SerializeField] private DownloadViewField m_View = new();
        [SerializeField] private ContentUpdatePolicy m_UpdatePolicy;
        [SerializeField] private AudioSource m_FailureSound;
        private CcdContentUpdater m_Updater;
        private ContentDownloadSession m_Session;
        private Coroutine m_Routine;
        private bool m_Accepted;
        private bool m_Retry;
        private Rect m_LastSafeArea;
        private Vector2Int m_LastScreen;
        private bool m_Prepared;
        private AsyncOperationHandle<SceneInstance> m_Scene;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            m_View.Bind(HandleActionInternal);
            FitScreenInternal();
        }
        private void Start() => BeginInternal();

        private void LateUpdate()
        {
            if (Screen.safeArea != m_LastSafeArea || Screen.width != m_LastScreen.x || Screen.height != m_LastScreen.y)
                FitScreenInternal();
        }

        private void FitScreenInternal()
        {
            m_LastSafeArea = Screen.safeArea;
            m_LastScreen = new Vector2Int(Screen.width, Screen.height);
            m_View.FitSafeArea(m_LastSafeArea, m_LastScreen.x, m_LastScreen.y);
        }

        private void HandleActionInternal()
        {
            if (m_Retry) BeginInternal();
            else m_Accepted = true;
        }

        private void BeginInternal()
        {
            if (m_Routine != null || m_Prepared) return;
            m_Retry = false;
            if (m_FailureSound != null) m_FailureSound.gameObject.SetActive(false);
            m_Session = new ContentDownloadSession();
            m_Routine = StartCoroutine(PrepareInternal());
        }

        private IEnumerator PrepareInternal()
        {
            m_View.ShowWaiting("Preparing AetherJump", "Checking game content…");
            yield return m_Session.Initialize();
            if (!m_Session.Succeeded) { FailInternal(m_Session.Error); yield break; }
            m_Updater?.Dispose();
            m_Updater = new CcdContentUpdater(m_UpdatePolicy, Application.persistentDataPath);
            if (m_UpdatePolicy != null && m_UpdatePolicy.Valid)
            {
                m_View.ShowWaiting("Checking for updates", "Checking for new game content…");
                yield return m_Updater.Check();
            }
            object downloadKey = m_UpdatePolicy != null && m_UpdatePolicy.Valid ? m_Updater.DownloadKey : m_DownloadLabel;
            yield return m_Session.Inspect(downloadKey);
            if (!m_Session.Succeeded) { FailInternal(m_Session.Error); yield break; }

            if (m_Session.RequiredBytes > 0)
            {
                if (!m_Accepted)
                {
                    m_View.ShowConfirmation(m_Session.RequiredBytes);
                    yield return new WaitUntil(() => m_Accepted);
                }
                m_View.ShowDownloading(m_Session.RequiredBytes);
                yield return m_Session.Download(downloadKey, UpdateProgressInternal);
                if (!m_Session.Succeeded)
                {
                    yield return m_Updater.UseCachedFallback();
                    object fallbackKey = m_Updater.Selection != null ? m_Updater.DownloadKey : m_DownloadLabel;
                    var cached = new ContentDownloadSession();
                    yield return cached.Inspect(fallbackKey);
                    if (!cached.Succeeded || cached.RequiredBytes != 0) { FailInternal(m_Session.Error); yield break; }
                    m_View.ShowWaiting("Using saved content", "The update can be retried next time.");
                }
            }

            m_View.ShowWaiting("Starting AetherJump", "Opening Home…");
            yield return OpenSceneInternal();
            if (m_Scene.Status != AsyncOperationStatus.Succeeded)
            {
                if (m_Scene.IsValid()) Addressables.Release(m_Scene);
                var failedVersion = m_Updater.Version;
                yield return m_Updater.UseCachedFallback();
                if (m_Updater.Version == failedVersion && m_Updater.Selection != null)
                    m_Updater.UseBaseline();
                var cached = new ContentDownloadSession();
                object fallbackKey = m_Updater.Selection != null && m_Updater.Version != failedVersion ? m_Updater.DownloadKey : m_DownloadLabel;
                yield return cached.Inspect(fallbackKey);
                if (!cached.Succeeded || cached.RequiredBytes != 0)
                { FailInternal("Could not open the game. Tap Retry."); yield break; }
                m_View.ShowWaiting("Using saved content", "Opening the saved game content…");
                yield return OpenSceneInternal();
                if (m_Scene.Status != AsyncOperationStatus.Succeeded)
                {
                    if (m_Scene.IsValid()) Addressables.Release(m_Scene);
                    m_Updater.Dispose();
                    FailInternal("Could not open the game. Tap Retry.");
                    yield break;
                }
            }
            // Addressables releases this scene handle when its scene is unloaded.
            // Keep the bootstrap owner alive to avoid cancelling activation during the single-scene switch.
            m_Prepared = true;
            m_Updater.MarkReady();
            m_Routine = null;
            m_View.Hide();
            Destroy(gameObject);
        }

        private IEnumerator OpenSceneInternal()
        {
            m_Updater.Activate();
            m_Scene = m_Updater.Selection?.Scene != null
                ? Addressables.LoadSceneAsync(m_Updater.Selection.Scene, LoadSceneMode.Single, true)
                : Addressables.LoadSceneAsync(m_GameSceneKey, LoadSceneMode.Single, true);
            yield return m_Scene;
        }

        private void UpdateProgressInternal(float progress)
        {
            m_View.SetProgress(m_Session.DownloadedBytes, m_Session.TotalBytes, progress);
        }

        private void FailInternal(string reason)
        {
            m_Routine = null;
            m_Retry = true;
            m_View.ShowFailure(reason);
            if (m_FailureSound != null) m_FailureSound.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            if (m_Routine != null) StopCoroutine(m_Routine);
            m_Updater?.Dispose();
        }
    }
}
