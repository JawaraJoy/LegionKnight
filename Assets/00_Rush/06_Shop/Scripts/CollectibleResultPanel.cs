using LegionKnight;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rush
{
    public class CollectibleResultPanel : PanelView
    {
        [SerializeField] private CollectibleResultItemPool m_ItemPool;
        [SerializeField] protected Button m_CloseButton;
        [SerializeField] private TextMeshProUGUI m_SpecialDropNoticeText;

        [SerializeField] private float m_SpawnInterval = 0.15f;
        [SerializeField] private float m_DelayBeforeFirst = 0.3f;

        [SerializeField] private AudioSource m_AudioSource;
        [SerializeField] private AudioClip m_SpawnSfx;
        [SerializeField] private AudioClip m_SpecialSpawnSfx;

        private const float c_ResultDoneDelay = 1f;
        private Coroutine m_SpawnCoroutine;
        private int m_SpawnVersion;

        [SerializeField]
        protected UnityEvent m_OnResultDone;
        public UnityEvent OnResultDone => m_OnResultDone;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (m_CloseButton != null)
            {
                m_CloseButton.onClick.RemoveListener(HideInternal);
                m_CloseButton.onClick.AddListener(HideInternal);
            }
        }

        protected override void HideInternal()
        {
            StopSpawnInternal();
            if (m_CloseButton != null) m_CloseButton.onClick.RemoveListener(HideInternal);
            m_ItemPool?.ReturnAll();
            base.HideInternal();
        }

        public void Show(CollectibleResultData result)
        {
            if (result == null) throw new System.ArgumentNullException(nameof(result));
            StopSpawnInternal();
            m_ItemPool?.ReturnAll();

            if (m_SpecialDropNoticeText != null)
                m_SpecialDropNoticeText.gameObject.SetActive(false);

            ShowInternal();
            if (!gameObject.activeInHierarchy || !IsShown) return;
            m_SpawnCoroutine = StartCoroutine(SpawnSequenceRoutine(result, m_SpawnVersion));
        }

        private IEnumerator SpawnSequenceRoutine(CollectibleResultData result, int version)
        {
            yield return new WaitForSecondsRealtime(m_DelayBeforeFirst);

            for (int i = 0; i < result.Entries.Count; i++)
            {
                if (version != m_SpawnVersion || !IsShown) yield break;
                var entry = result.Entries[i];
                bool isLast = i == result.Entries.Count - 1;

                SpawnEntryInternal(entry);
                PlaySfxInternal(entry);

                if (isLast && result.WasSpecialDrop && m_SpecialDropNoticeText != null)
                    m_SpecialDropNoticeText.gameObject.SetActive(true);

                if (!isLast)
                    yield return new WaitForSecondsRealtime(m_SpawnInterval);
            }

            yield return new WaitForSecondsRealtime(c_ResultDoneDelay);
            if (version != m_SpawnVersion || !IsShown) yield break;
            m_SpawnCoroutine = null;
            OnResultDoneInvoke();
        }

        private void SpawnEntryInternal(CollectibleResultEntry entry)
        {
            if (m_ItemPool == null) return;
            var ui = m_ItemPool.Rent();
            ui.Setup(entry);
        }

        private void PlaySfxInternal(CollectibleResultEntry entry)
        {
            if (m_AudioSource == null) return;
            AudioClip clip = IsSpecialEntryInternal(entry) && m_SpecialSpawnSfx != null
                ? m_SpecialSpawnSfx
                : m_SpawnSfx;
            if (clip != null) m_AudioSource.PlayOneShot(clip);
        }

        // Override di subclass untuk define apa yang dianggap "special"
        // Default: tidak ada yang special (shop tidak pakai ini)
        protected virtual bool IsSpecialEntryInternal(CollectibleResultEntry entry) => false;

        private void OnDisable()
        {
            StopSpawnInternal();
        }

        private void OnResultDoneInvoke() => m_OnResultDone?.Invoke();

        private void StopSpawnInternal()
        {
            m_SpawnVersion++;
            if (m_SpawnCoroutine == null) return;
            StopCoroutine(m_SpawnCoroutine);
            m_SpawnCoroutine = null;
        }
    }
}