using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class QuestTaskItemUI : MonoBehaviour, IUpdater
    {
        [Header("Task Info")]
        [SerializeField] private TextMeshProUGUI m_TaskNameText;
        [SerializeField] private TextMeshProUGUI m_TaskDescText;
        [SerializeField] private TextMeshProUGUI m_ProgressText;  // e.g. "3/5"
        [SerializeField] private Slider m_ProgressSlider;

        [Header("Reward")]
        [SerializeField] private Image m_RewardIcon;
        [SerializeField] private TextMeshProUGUI m_RewardAmountText;

        [Header("Claim Button")]
        [SerializeField] private Button m_ClaimButton;
        [SerializeField] private TextMeshProUGUI m_ClaimButtonText;

        [Header("States")]
        [SerializeField] private GameObject m_StateInProgress;
        [SerializeField] private GameObject m_StateComplete;   // complete, not yet claimed
        [SerializeField] private GameObject m_StateClaimed;    // reward claimed

        [Header("Reset Countdown")]
        [SerializeField] private GameObject m_CountdownGroup;
        [SerializeField] private TextMeshProUGUI m_CountdownText;

        private QuestTaskConfig m_Task;
        private bool m_ShowCountdown;

        // ── IUpdater ──────────────────────────────────────────────────────────

        public bool IsActive => gameObject.activeInHierarchy && m_ShowCountdown;

        public void Tick()
        {
            var state = RushPlayer.Instance.QuestManager.GetTaskState(m_Task);
            RefreshCountdownInternal(state);
        }

        // ── Lifecycle ─────────────────────────────────────────────────────────

        private void OnEnable()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }

        private void OnDisable()
        {
            m_ShowCountdown = false;
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        // ── Setup ─────────────────────────────────────────────────────────────

        public void Setup(QuestTaskState state)
        {
            m_Task = state.Config;

            if (m_TaskNameText != null) m_TaskNameText.text = state.Config.BaseInfo.Name;
            if (m_TaskDescText != null) m_TaskDescText.text = state.Config.BaseInfo.Description;

            RefreshRewardInternal(state.Config);
            RefreshProgressInternal(state);
            RefreshStateInternal(state);
            RefreshCountdownInternal(state);

            if (m_ClaimButton != null)
            {
                m_ClaimButton.onClick.RemoveAllListeners();
                m_ClaimButton.onClick.AddListener(OnClaimClickedInternal);
            }
        }

        public void Refresh(QuestTaskState state)
        {
            RefreshProgressInternal(state);
            RefreshStateInternal(state);
            RefreshCountdownInternal(state);
        }

        // ── Refresh ───────────────────────────────────────────────────────────

        private void RefreshRewardInternal(QuestTaskConfig config)
        {
            if (m_RewardIcon != null)
                m_RewardIcon.sprite = config.RewardCollectible?.CollectibleField?.Icon;

            if (m_RewardAmountText != null)
            {
                bool show = config.RewardAmount >= 2;
                m_RewardAmountText.gameObject.SetActive(show);
                if (show) m_RewardAmountText.text = $"x{config.RewardAmount}";
            }
        }

        private void RefreshProgressInternal(QuestTaskState state)
        {
            if (m_ProgressText != null)
                m_ProgressText.text = $"{state.CurrentCount}/{state.TargetCount}";

            if (m_ProgressSlider != null)
            {
                m_ProgressSlider.minValue = 0;
                m_ProgressSlider.maxValue = state.TargetCount;
                m_ProgressSlider.value = state.CurrentCount;
            }
        }

        private void RefreshStateInternal(QuestTaskState state)
        {
            if (m_StateInProgress != null)
                m_StateInProgress.SetActive(!state.IsComplete && !state.IsClaimed);
            if (m_StateComplete != null)
                m_StateComplete.SetActive(state.CanClaim);
            if (m_StateClaimed != null)
                m_StateClaimed.SetActive(state.IsClaimed);

            if (m_ClaimButton != null)
                m_ClaimButton.interactable = state.CanClaim;

            if (m_ClaimButtonText != null)
                m_ClaimButtonText.text = state.IsClaimed ? "Claimed" : "Claim";
        }

        private void RefreshCountdownInternal(QuestTaskState state)
        {
            // Show countdown only after claimed — tells player when task resets
            m_ShowCountdown = state.IsClaimed && state.SecondsUntilReset > 0;

            if (m_CountdownGroup != null) m_CountdownGroup.SetActive(m_ShowCountdown);
            if (!m_ShowCountdown || m_CountdownText == null) return;

            m_CountdownText.text = FormatCountdownInternal(state.SecondsUntilReset);
        }

        // ── Callbacks ─────────────────────────────────────────────────────────

        private void OnClaimClickedInternal() =>
            RushPlayer.Instance.QuestManager.Claim(m_Task);

        // ── Helpers ───────────────────────────────────────────────────────────

        private static string FormatCountdownInternal(double totalSeconds)
        {
            var span = System.TimeSpan.FromSeconds(totalSeconds);
            return span.Hours > 0
                ? $"{span.Hours:D2}:{span.Minutes:D2}:{span.Seconds:D2}"
                : $"{span.Minutes:D2}:{span.Seconds:D2}";
        }
    }
}