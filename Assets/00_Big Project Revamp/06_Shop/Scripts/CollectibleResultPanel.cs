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

        [Header("Spawn Sequence")]
        [SerializeField] private float m_SpawnInterval = 0.15f;
        [SerializeField] private float m_DelayBeforeFirst = 0.3f;

        [Header("SFX")]
        [SerializeField] private AudioSource m_AudioSource;
        [SerializeField] private AudioClip m_SpawnSfx;
        [SerializeField] private AudioClip m_SpecialSpawnSfx;

        private Coroutine m_SpawnCoroutine;

        [SerializeField]
        protected UnityEvent m_OnResultDone;
        public UnityEvent OnResultDone => m_OnResultDone;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (m_CloseButton != null) m_CloseButton.onClick.AddListener(Hide);
        }

        protected override void HideInternal()
        {
            StopSpawnInternal();
            if (m_CloseButton != null) m_CloseButton.onClick.RemoveListener(Hide);
            m_ItemPool?.ReturnAll();
            base.HideInternal();
        }

        public void Show(CollectibleResultData result)
        {
            m_ItemPool?.ReturnAll();

            if (m_SpecialDropNoticeText != null)
                m_SpecialDropNoticeText.gameObject.SetActive(false);

            Show();
            StopSpawnInternal();
            m_SpawnCoroutine = StartCoroutine(SpawnSequenceRoutine(result));
        }

        private IEnumerator SpawnSequenceRoutine(CollectibleResultData result)
        {
            yield return new WaitForSeconds(m_DelayBeforeFirst);

            for (int i = 0; i < result.Entries.Count; i++)
            {
                var entry = result.Entries[i];
                bool isLast = i == result.Entries.Count - 1;

                SpawnEntryInternal(entry);
                PlaySfxInternal(entry);

                if (isLast && result.WasSpecialDrop && m_SpecialDropNoticeText != null)
                    m_SpecialDropNoticeText.gameObject.SetActive(true);

                if (!isLast)
                    yield return new WaitForSeconds(m_SpawnInterval);
            }

            m_SpawnCoroutine = null;
            yield return new WaitForSeconds(1f);
            m_OnResultDone?.Invoke();
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

        private void StopSpawnInternal()
        {
            if (m_SpawnCoroutine == null) return;
            StopCoroutine(m_SpawnCoroutine);
            m_SpawnCoroutine = null;
        }
    }
}