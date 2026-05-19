// AdRewardButton.cs
using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    /// <summary>
    /// Tombol reward harian via iklan.
    /// - Menampilkan icon + jumlah reward
    /// - Hanya bisa ditekan 1x per hari (reset tengah malam)
    /// - Setelah iklan selesai, beri reward dan tampilkan CollectibleResultPanel
    /// </summary>
    public class DailyAdRewardButton : MonoBehaviour
    {
        // ── Config ────────────────────────────────────────────────────────────────

        [Header("Reward")]
        [SerializeField] private CollectibleConfig m_Collectible;
        [SerializeField] private int m_Amount = 1;

        [Header("UI")]
        [SerializeField] private Button m_Button;
        [SerializeField] private Image m_RewardIcon;
        [SerializeField] private TextMeshProUGUI m_AmountText;
        [SerializeField] private TextMeshProUGUI m_CooldownText;   // opsional — tampilkan "Besok" saat cooldown
        [SerializeField] private GameObject m_ReadyState;          // aktif saat bisa diklaim
        [SerializeField] private GameObject m_CooldownState;       // aktif saat sudah diklaim hari ini
        [SerializeField]
        private UnityEvent m_OnRewardClaimed; // opsional — event tambahan saat reward diklaim (misal untuk analytics)

        // ── Persistence ───────────────────────────────────────────────────────────

        // Key unik per GameObject — bisa override di Inspector jika ada beberapa button berbeda
        [Header("Persistence")]
        [SerializeField] private string m_ButtonId = "adrewardbtn_default";

        private string LastClaimKey => $"{m_ButtonId}_lastclaim";

        // ── Result panel cache ────────────────────────────────────────────────────

        private ShopResultPanel m_ResultPanel;
        private ShopResultPanel ResultPanel
        {
            get
            {
                if (m_ResultPanel == null)
                    m_ResultPanel = CanvasManager.Instance.GetPanel<ShopResultPanel>();
                return m_ResultPanel;
            }
        }

        // ── Lifecycle ─────────────────────────────────────────────────────────────

        private void Start()
        {
            RefreshIcon();
            RefreshState();
            m_Button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDestroy()
        {
            m_Button.onClick.RemoveListener(OnButtonClicked);
        }

        // Refresh state saat panel dibuka (panggil dari OnShow jika perlu)
        public void Refresh()
        {
            RefreshState();
        }

        // ── UI ────────────────────────────────────────────────────────────────────

        private void RefreshIcon()
        {
            if (m_Collectible == null) return;

            Sprite icon = GetIconFromCollectible(m_Collectible);
            if (m_RewardIcon != null && icon != null)
                m_RewardIcon.sprite = icon;

            if (m_AmountText != null)
                m_AmountText.text = $"+{m_Amount}";
        }

        private void RefreshState()
        {
            bool ready = IsReadyToday();

            m_Button.interactable = ready;

            if (m_ReadyState != null) m_ReadyState.SetActive(ready);
            if (m_CooldownState != null) m_CooldownState.SetActive(!ready);

            if (m_CooldownText != null)
                m_CooldownText.text = ready ? string.Empty : "Come back Tomorrow";
        }

        // ── Button handler ────────────────────────────────────────────────────────

        private void OnButtonClicked()
        {
            if (!IsReadyToday()) return;

            UnityService.Instance.ShowRewardedAd(OnAdCompleted);
        }

        private void OnAdCompleted()
        {
            // Catat waktu klaim
            SaveClaimTime();

            // Beri reward ke player
            CollectibleControl.AddCollectibleStatic("ad_reward_button", m_Collectible, m_Amount);

            // Tampilkan result panel
            CollectibleResultData data = BuildResultData();
            ResultPanel.Show(data);

            // Update UI button
            RefreshState();
            // Event tambahan (misal untuk analytics)
            m_OnRewardClaimed?.Invoke();
        }

        // ── Daily cooldown ────────────────────────────────────────────────────────

        private bool IsReadyToday()
        {
            if (!UnityService.Instance.HasData(LastClaimKey))
                return true;

            string savedDate = UnityService.Instance.GetData<string>(LastClaimKey);

            // Cek apakah tanggal simpan berbeda dengan hari ini
            if (System.DateTime.TryParse(savedDate, out System.DateTime lastClaim))
                return lastClaim.Date < System.DateTime.Now.Date;

            return true;
        }

        private void SaveClaimTime()
        {
            string today = System.DateTime.Now.ToString("o"); // ISO 8601
            UnityService.Instance.SaveData(LastClaimKey, today);
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private CollectibleResultData BuildResultData()
        {
            var data = new CollectibleResultData();
            data.AddEntry(m_Collectible, m_Amount);
            return data;
        }

        private static Sprite GetIconFromCollectible(CollectibleConfig config)
        {
            return config switch
            {
                ItemConfig item => item.CollectibleField.Icon,
                HeroUnitConfig hero => hero.CollectibleField.Icon,
                PlatformConfig platform => platform.CollectibleField.Icon,
                EnergyConfig energy => energy.CollectibleField.Icon,
                _ => null
            };
        }
    }
}