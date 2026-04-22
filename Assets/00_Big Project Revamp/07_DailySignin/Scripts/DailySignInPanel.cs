using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    public class DailySignInPanel : PanelView, IUpdater
    {
        // Assign manually in inspector — one entry per reward day
        // Array length should match DailySignInConfig.Rewards length
        [SerializeField] private DailySignInDayItemUI[] m_DayItems;

        [SerializeField] private Button m_ClaimButton;
        [SerializeField] private TextMeshProUGUI m_ClaimButtonText;
        [SerializeField] private Button m_CloseButton;

        [Header("Status")]
        [SerializeField] private TextMeshProUGUI m_StatusText;
        [SerializeField] private TextMeshProUGUI m_CountdownText;
        [SerializeField] private GameObject m_CountdownGroup;

        private DailySignInManager Manager => RushPlayer.Instance.DailySignInManager;

        // ── IUpdater ──────────────────────────────────────────────────────────

        public bool IsActive => IsShow;

        public void Tick()
        {
            var state = Manager.GetState();
            RefreshCountdownInternal(state);
        }

        // ── Lifecycle ─────────────────────────────────────────────────────────

        protected override void ShowInternal()
        {
            base.ShowInternal();
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);

            if (m_ClaimButton != null) m_ClaimButton.onClick.AddListener(OnClaimClickedInternal);
            if (m_CloseButton != null) m_CloseButton.onClick.AddListener(Hide);

            Manager.OnClaimSuccess.AddListener(OnClaimSuccessInternal);
            Manager.OnClaimFailed.AddListener(OnClaimFailedInternal);
            Manager.OnCycleReset.AddListener(OnCycleResetInternal);

            RefreshViewInternal();
        }

        protected override void HideInternal()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);

            if (m_ClaimButton != null) m_ClaimButton.onClick.RemoveListener(OnClaimClickedInternal);
            if (m_CloseButton != null) m_CloseButton.onClick.RemoveListener(Hide);

            Manager.OnClaimSuccess.RemoveListener(OnClaimSuccessInternal);
            Manager.OnClaimFailed.RemoveListener(OnClaimFailedInternal);
            Manager.OnCycleReset.RemoveListener(OnCycleResetInternal);

            base.HideInternal();
        }

        // ── View ──────────────────────────────────────────────────────────────

        private void RefreshViewInternal()
        {
            var state = Manager.GetState();
            var rewards = Manager.Config.Rewards;

            if (m_DayItems != null && rewards != null)
            {
                for (int i = 0; i < m_DayItems.Length; i++)
                {
                    if (m_DayItems[i] == null) continue;

                    // If inspector has more slots than config rewards, hide the extra
                    if (i >= rewards.Length)
                    {
                        m_DayItems[i].gameObject.SetActive(false);
                        continue;
                    }

                    m_DayItems[i].gameObject.SetActive(true);
                    m_DayItems[i].Setup(i, rewards[i], GetDayDisplayStateInternal(i, state));
                }
            }

            RefreshClaimButtonInternal(state);
            RefreshStatusTextInternal(state);
            RefreshCountdownInternal(state);
        }

        private DayDisplayState GetDayDisplayStateInternal(int dayIndex,
            DailySignInState state)
        {
            if (dayIndex < state.CurrentDay)
                return DayDisplayState.Claimed;

            if (dayIndex == state.CurrentDay)
            {
                if (state.CycleComplete) return DayDisplayState.Complete;
                if (state.CanClaimToday) return DayDisplayState.Available;
                return DayDisplayState.Claimed;
            }

            return state.CycleComplete
                ? DayDisplayState.Complete
                : DayDisplayState.Locked;
        }

        private void RefreshClaimButtonInternal(DailySignInState state)
        {
            if (m_ClaimButton == null) return;
            m_ClaimButton.interactable = state.CanClaimToday;

            if (m_ClaimButtonText != null)
            {
                m_ClaimButtonText.text = state.CycleComplete
                    ? "Completed"
                    : state.CanClaimToday ? "Claim" : "Come back tomorrow";
            }
        }

        private void RefreshStatusTextInternal(DailySignInState state)
        {
            if (m_StatusText == null) return;

            if (state.CycleComplete && !Manager.Config.LoopOnComplete)
                m_StatusText.text = "All rewards claimed! See you next cycle.";
            else if (state.CanClaimToday)
                m_StatusText.text = $"Day {state.CurrentDay + 1} reward is ready!";
            else
                m_StatusText.text = $"Day {state.CurrentDay} claimed. Next reward tomorrow.";
        }

        private void RefreshCountdownInternal(DailySignInState state)
        {
            bool showCountdown = !state.CanClaimToday && state.SecondsUntilReset > 0;

            if (m_CountdownGroup != null) m_CountdownGroup.SetActive(showCountdown);
            if (!showCountdown || m_CountdownText == null) return;

            m_CountdownText.text = FormatCountdownInternal(state.SecondsUntilReset);
        }

        // ── Callbacks ─────────────────────────────────────────────────────────

        private void OnClaimClickedInternal() => Manager.Claim();

        private void OnClaimSuccessInternal(CollectibleResultData result)
        {
            RefreshViewInternal();
        }

        private void OnClaimFailedInternal(string message) =>
            Debug.LogWarning($"[DailySignIn] {message}");

        private void OnCycleResetInternal() => RefreshViewInternal();

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