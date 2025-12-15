using MoreMountains.Tools;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public class FakeDownloadContent : PanelView
    {
        [Header("Download Settings")]
        [SerializeField] private float m_MinDownloadTime = 3f;
        [SerializeField] private float m_MaxDownloadTime = 8f;
        [SerializeField] private float m_SizeInMB = 50f;

        [Header("UI")]
        [SerializeField] private Slider m_ProgressBar;
        [SerializeField] private TextMeshProUGUI m_ProgressText;

        [Header("Events")]
        [SerializeField] private UnityEvent m_OnDownloadCompleted;

        [SerializeField, MMReadOnly]
        private bool m_IsDownloaded = false;

        private Coroutine m_DownloadRoutine;

        private readonly string m_IsDownloadedKey = "isdownloadfake";
        protected override void ShowBindingInternal(string uniqueId)
        {
            bool hasBeenDownloadedKey = UnityService.Instance.HasData(m_IsDownloadedKey);
            if (hasBeenDownloadedKey)
            {
                m_IsDownloaded = UnityService.Instance.GetData<bool>(m_IsDownloadedKey);
            }
            if (m_IsDownloaded) return;
            base.ShowBindingInternal(uniqueId);
        }
        protected override void OnShowInvoke()
        {
            base.OnShowInvoke();
            StartFakeDownload();
        }

        private void StartFakeDownload()
        {
            if (m_DownloadRoutine != null)
                StopCoroutine(m_DownloadRoutine);

            m_DownloadRoutine = StartCoroutine(FakeDownloadRoutine());
        }

        private IEnumerator FakeDownloadRoutine()
        {
            float downloadDuration = Random.Range(m_MinDownloadTime, m_MaxDownloadTime);
            float elapsedTime = 0f;

            m_ProgressBar.value = 0f;

            while (elapsedTime < downloadDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / downloadDuration);

                float downloadedMB = progress * m_SizeInMB;

                m_ProgressBar.value = progress;
                m_ProgressText.text = $"{downloadedMB:0.0} MB / {m_SizeInMB} MB";

                yield return null;
            }

            CompleteDownload();
        }

        private void CompleteDownload()
        {
            m_ProgressBar.value = 1f;
            m_ProgressText.text = $"{m_SizeInMB} MB / {m_SizeInMB} MB";
            m_IsDownloaded = true;

            m_OnDownloadCompleted?.Invoke();
        }
    }
}
