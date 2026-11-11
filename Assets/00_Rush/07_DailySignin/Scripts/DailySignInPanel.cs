using System;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class DailySignInPanel : PanelView, IUpdater
    {
        [SerializeField] private DailySignInDayItemUI[] m_DayItems;
        [SerializeField] private Button m_CloseButton;
        [SerializeField] private TextMeshProUGUI m_StatusText;
        [SerializeField] private TextMeshProUGUI m_CountdownText;
        [SerializeField] private GameObject m_CountdownGroup;
        [SerializeField] private TextMeshProUGUI m_ClaimedCount;
        [SerializeField] private RectTransform m_Dialog;

        private DailySignInManager m_Manager;
        private DailySignInState m_Displayed;
        private long m_LastSecond = -1;

        public bool IsActive => IsShown;

        public void Tick()
        {
            long second = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            if (second == m_LastSecond || m_Manager == null) return;
            m_LastSecond = second;
            DailySignInState state = m_Manager.GetState();
            if (m_Displayed == null || state.CycleStartTime != m_Displayed.CycleStartTime
                || state.CurrentDay != m_Displayed.CurrentDay || state.CanClaimToday != m_Displayed.CanClaimToday)
            {
                RefreshViewInternal(state);
                return;
            }
            RefreshCountdownInternal(state);
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (!IsShown) return;
            if (m_Manager == null)
            {
                m_Manager = RushPlayer.Instance.DailySignInManager;
                m_Manager.OnClaimSuccess.AddListener(OnClaimSuccessInternal);
                m_Manager.OnClaimFailed.AddListener(OnClaimFailedInternal);
                m_Manager.OnCycleReset.AddListener(OnCycleResetInternal);
                if (m_CloseButton != null) m_CloseButton.onClick.AddListener(Hide);
                UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
            }
            FitDialogInternal();
            RefreshViewInternal(m_Manager.GetState());
        }

        protected override void HideInternal()
        {
            if (m_Manager != null)
            {
                UpdateBank.Instance.UnregisterUpdateTick(gameObject);
                DisconnectInternal();
            }
            base.HideInternal();
        }

        private void OnDestroy() => DisconnectInternal();

        private void DisconnectInternal()
        {
            if (m_CloseButton != null) m_CloseButton.onClick.RemoveListener(Hide);
            if (m_Manager == null) return;
            m_Manager.OnClaimSuccess.RemoveListener(OnClaimSuccessInternal);
            m_Manager.OnClaimFailed.RemoveListener(OnClaimFailedInternal);
            m_Manager.OnCycleReset.RemoveListener(OnCycleResetInternal);
            m_Manager = null;
            m_Displayed = null;
        }

        private void OnRectTransformDimensionsChange() => FitDialogInternal();

        private void FitDialogInternal()
        {
            if (m_Dialog == null || !(m_Dialog.parent is RectTransform stage)) return;
            // Leave room for the pennant above the dialog and the dismiss hint below it.
            float scale = Mathf.Min(1f, stage.rect.width / 420f, stage.rect.height / 710f);
            if (scale > 0f) m_Dialog.localScale = new Vector3(scale, scale, 1f);
        }

        private void RefreshViewInternal(DailySignInState state)
        {
            m_Displayed = state;
            DailySignInRewardEntry[] rewards = m_Manager.Config.Rewards;
            int claimedCount = 0;
            for (int i = 0; i < state.TotalDays; i++)
            {
                bool claimed = m_Manager.IsDayClaimed(i);
                if (claimed) claimedCount++;
                if (i >= m_DayItems.Length || m_DayItems[i] == null) continue;
                DailySignInDayItemUI item = m_DayItems[i];
                item.gameObject.SetActive(true);
                DayDisplayState display = DayDisplayState.Locked;
                if (claimed) display = DayDisplayState.Claimed;
                else if (i < state.CurrentDay) display = DayDisplayState.Missed;
                else if (i == state.CurrentDay && state.CanClaimToday) display = DayDisplayState.Available;
                int day = i;
                DailySignInManager manager = m_Manager;
                DateTime cycleStart = state.CycleStartTime;
                item.Setup(day, rewards[i], display,
                    () => manager.Claim(day, cycleStart),
                    () => OnMissedClaimClickedInternal(manager, day, cycleStart));
            }
            for (int i = state.TotalDays; i < m_DayItems.Length; i++)
            {
                if (m_DayItems[i] != null) m_DayItems[i].gameObject.SetActive(false);
            }

            if (m_ClaimedCount != null) m_ClaimedCount.text = $"{claimedCount} / {state.TotalDays}";
            if (m_StatusText != null)
            {
                if (state.CanClaimToday) m_StatusText.text = $"Day {state.CurrentDay + 1} reward is ready!";
                else m_StatusText.text = state.CycleComplete ? "Next cycle" : "Next check-in";
            }
            RefreshCountdownInternal(state);
        }

        private static void OnMissedClaimClickedInternal(DailySignInManager manager, int day, DateTime cycleStart)
        {
            DailySignInState state = manager.GetState();
            if (state.CycleStartTime != cycleStart || day >= state.CurrentDay || manager.IsDayClaimed(day)) return;
            UnityService.Instance.ShowRewardedAd(() =>
            {
                if (manager != null) manager.ClaimMissedDay(day, cycleStart);
            });
        }

        private void RefreshCountdownInternal(DailySignInState state)
        {
            if (m_CountdownGroup != null) m_CountdownGroup.SetActive(!state.CanClaimToday);
            if (state.CanClaimToday || m_CountdownText == null) return;
            double seconds = Math.Max(0, Math.Ceiling((state.NextRewardTime - DateTime.Now).TotalSeconds));
            TimeSpan span = TimeSpan.FromSeconds(seconds);
            string clock = $"{span.Hours:D2}:{span.Minutes:D2}:{span.Seconds:D2}";
            m_CountdownText.text = span.Days > 0 ? $"{span.Days}d {clock}" : clock;
        }

        private void OnClaimSuccessInternal(CollectibleResultData result)
        {
            RefreshViewInternal(m_Manager.GetState());
            CanvasManager.Instance.GetPanel<ShopResultPanel>()?.Show(result);
        }

        private void OnClaimFailedInternal(string message)
        {
            Debug.LogWarning($"[DailySignIn] {message}");
            RefreshViewInternal(m_Manager.GetState());
        }

        private void OnCycleResetInternal() => m_Displayed = null;
    }
}
