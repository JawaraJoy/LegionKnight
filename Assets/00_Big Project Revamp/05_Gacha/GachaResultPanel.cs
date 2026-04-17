using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    public class GachaResultPanel : PanelView
    {
        [SerializeField] private GachaResultItemPool m_ResultItemPool;
        [SerializeField] private Button m_CloseButton;
        [SerializeField] private TextMeshProUGUI m_PityNoticeText;

        [Header("Spawn Sequence")]
        [SerializeField] private float m_SpawnInterval = 0.15f;
        [SerializeField] private float m_DelayBeforeFirst = 0.3f;

        [Header("SFX")]
        [SerializeField] private AudioSource m_AudioSource;
        [SerializeField] private AudioClip m_SpawnSfx;
        // opsional: sfx khusus jika item yang spawn adalah pity
        [SerializeField] private AudioClip m_PitySpawnSfx;

        private Coroutine m_SpawnCoroutine;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (m_CloseButton != null) m_CloseButton.onClick.AddListener(Hide);
        }

        protected override void HideInternal()
        {
            StopSpawnInternal();
            if (m_CloseButton != null) m_CloseButton.onClick.RemoveListener(Hide);
            m_ResultItemPool?.ReturnAll();
            base.HideInternal();
        }

        public void Show(GachaDrawResult result)
        {
            // return semua item lama sebelum mulai sequence baru
            m_ResultItemPool?.ReturnAll();
            if (m_PityNoticeText != null)
                m_PityNoticeText.gameObject.SetActive(false);

            Show();
            StopSpawnInternal();
            m_SpawnCoroutine = RushGameManager.Instance.StartCoroutine(SpawnSequenceRoutine(result));
        }

        private IEnumerator SpawnSequenceRoutine(GachaDrawResult result)
        {
            yield return new WaitForSeconds(m_DelayBeforeFirst);

            for (int i = 0; i < result.Items.Count; i++)
            {
                var collectable = result.Items[i];
                bool isLast = i == result.Items.Count - 1;

                SpawnItemInternal(collectable);
                PlaySpawnSfxInternal(collectable);

                // tampilkan notice pity setelah item terakhir jika triggered
                if (isLast && result.WasPityTriggered && m_PityNoticeText != null)
                    m_PityNoticeText.gameObject.SetActive(true);

                if (!isLast)
                    yield return new WaitForSeconds(m_SpawnInterval);
            }

            m_SpawnCoroutine = null;
        }

        private void SpawnItemInternal(GachaCollectableConfig collectable)
        {
            if (m_ResultItemPool == null) return;
            var ui = m_ResultItemPool.Rent();
            ui.Setup(collectable);
        }

        private void PlaySpawnSfxInternal(GachaCollectableConfig collectable)
        {
            if (m_AudioSource == null) return;

            // cek apakah item ini bagian dari guarantee array banner aktif
            // untuk menentukan apakah pakai sfx pity atau normal
            bool isPityItem = IsPityItemInternal(collectable);
            AudioClip clip = isPityItem && m_PitySpawnSfx != null
                ? m_PitySpawnSfx
                : m_SpawnSfx;

            if (clip != null)
                m_AudioSource.PlayOneShot(clip);
        }

        private bool IsPityItemInternal(GachaCollectableConfig collectable)
        {
            var banner = RushPlayer.Instance.GachaManager.ActiveBanner;
            if (banner == null) return false;

            if (ContainsInArrayInternal(banner.FinalPityGuarantees, collectable)) return true;
            if (ContainsInArrayInternal(banner.SmallPityGuarantees, collectable)) return true;
            if (ContainsInArrayInternal(banner.FirstDrawGuarantees, collectable)) return true;

            return false;
        }

        private static bool ContainsInArrayInternal(
            GachaCollectableConfig[] arr, GachaCollectableConfig target)
        {
            if (arr == null) return false;
            foreach (var c in arr)
                if (c == target) return true;
            return false;
        }

        private void StopSpawnInternal()
        {
            if (m_SpawnCoroutine == null) return;
            StopCoroutine(m_SpawnCoroutine);
            m_SpawnCoroutine = null;
        }
    }
}