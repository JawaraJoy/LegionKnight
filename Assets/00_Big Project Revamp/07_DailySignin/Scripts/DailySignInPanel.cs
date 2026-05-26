using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    public class DailySignInPanel : PanelView, IUpdater
    {
        [SerializeField] private DailySignInDayItemUI[] m_DayItems;

        [SerializeField] private Button m_CloseButton;

        [Header("Status")]
        [SerializeField] private TextMeshProUGUI m_StatusText;
        [SerializeField] private TextMeshProUGUI m_CountdownText;
        [SerializeField] private GameObject m_CountdownGroup;

        private DailySignInManager Manager => RushPlayer.Instance.DailySignInManager;

        // ── IUpdater ──────────────────────────────────────────────────────────

        public bool IsActive => IsShown;

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

            if (m_CloseButton != null) m_CloseButton.onClick.AddListener(Hide);

            Manager.OnClaimSuccess.AddListener(OnClaimSuccessInternal);
            Manager.OnClaimFailed.AddListener(OnClaimFailedInternal);
            Manager.OnCycleReset.AddListener(OnCycleResetInternal);

            RefreshViewInternal();
        }

        protected override void HideInternal()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);

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

                    if (i >= rewards.Length)
                    {
                        m_DayItems[i].gameObject.SetActive(false);
                        continue;
                    }

                    m_DayItems[i].gameObject.SetActive(true);
                    m_DayItems[i].Setup(i, rewards[i],
                        GetDayDisplayStateInternal(i, state),
                        OnItemClaimClickedInternal,
                        () => OnMissedClaimClickedInternal(i));
                }
            }

            RefreshStatusTextInternal(state);
            RefreshCountdownInternal(state);
        }
        private void OnMissedClaimClickedInternal(int dayIndex)
        {
            UnityService.Instance.ShowRewardedAd(() =>
            {
                Manager.ClaimMissedDay(dayIndex);
            });
        }

        // Only refresh states without re-assigning all data
        private void RefreshDayStatesInternal()
        {
            var state = Manager.GetState();
            var rewards = Manager.Config.Rewards;

            if (m_DayItems == null || rewards == null) return;

            for (int i = 0; i < m_DayItems.Length && i < rewards.Length; i++)
            {
                if (m_DayItems[i] == null) continue;
                m_DayItems[i].RefreshState(GetDayDisplayStateInternal(i, state));
            }

            RefreshStatusTextInternal(state);
            RefreshCountdownInternal(state);
        }

        private DayDisplayState GetDayDisplayStateInternal(int dayIndex, DailySignInState state)
        {
            // 1. Hari yang SUDAH di-claim
            if (dayIndex < state.CurrentDay)
                return DayDisplayState.Claimed;

            // 2. Hari saat ini (target berikutnya)
            if (dayIndex == state.CurrentDay)
            {
                // Kalau cycle selesai
                if (state.CycleComplete)
                    return DayDisplayState.Complete;

                // Kalau boleh claim sekarang
                if (state.CanClaimToday)
                    return DayDisplayState.Available;

                // ❗ FIX UTAMA:
                // Kalau belum waktunya → HARUS LOCKED (bukan Claimed)
                return DayDisplayState.Locked;
            }

            // 3. Hari ke depan (belum nyampe)
            return state.CycleComplete
                ? DayDisplayState.Complete
                : DayDisplayState.Locked;
        }

        private void RefreshStatusTextInternal(DailySignInState state)
        {
            if (m_StatusText == null) return;

            if (state.CycleComplete && !Manager.Config.LoopOnComplete)
                m_StatusText.text = "All rewards claimed! See you next cycle.";
            else if (state.CanClaimToday)
                m_StatusText.text = $"Day {state.CurrentDay + 1} reward is ready!";
            else
                m_StatusText.text = $"Day {state.CurrentDay} claimed. Next reward in: ";
        }

        private void RefreshCountdownInternal(DailySignInState state)
        {
            bool showCountdown = !state.CanClaimToday && state.SecondsUntilReset > 0;

            if (m_CountdownGroup != null) m_CountdownGroup.SetActive(showCountdown);
            if (!showCountdown || m_CountdownText == null) return;

            m_CountdownText.text = FormatCountdownInternal(state.SecondsUntilReset);
        }

        // ── Callbacks ─────────────────────────────────────────────────────────

        // Called by whichever DayItemUI is in Available state
        private void OnItemClaimClickedInternal() => Manager.Claim();

        private void OnClaimSuccessInternal(CollectibleResultData result)
        {
            RefreshDayStatesInternal();
            var resultPanel = CanvasManager.Instance.GetPanel<ShopResultPanel>();
            resultPanel?.Show(result);
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