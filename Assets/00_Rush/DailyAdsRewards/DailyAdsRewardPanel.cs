using System;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class DailyAdsRewardPanel : PanelView, IUpdater
    {
        [Serializable]
        private sealed class HeaderField
        {
            [SerializeField] private TextMeshProUGUI m_ClaimTally;
            [SerializeField] private RectTransform m_ProgressFill;

            public void SetProgress(int claimed, int total)
            {
                if (m_ClaimTally != null) m_ClaimTally.text = $"{claimed} / {total} claimed";
                if (m_ProgressFill == null) return;
                float fraction = total > 0 ? (float)claimed / total : 0f;
                m_ProgressFill.anchorMax = new Vector2(fraction, m_ProgressFill.anchorMax.y);
                m_ProgressFill.gameObject.SetActive(claimed > 0);
            }
        }

        [SerializeField] private TextMeshProUGUI m_ResetTimerText;
        [SerializeField] private Button m_CloseButton;
        [SerializeField] private DailyAdsBundleView[] m_BundleViews;
        [SerializeField] private HeaderField m_Header = new();
        [SerializeField] private RectTransform m_DialogFit;

        private const float c_LayoutWidth = 424f;
        private const float c_LayoutHeight = 700f;
        private DailyAdsBundleManager m_Manager;
        private DateTime m_DisplayedDate;
        private int m_LastMinutes = -1;

        public DailyAdsBundleView[] BundleViews => m_BundleViews;
        public bool IsActive => m_Content != null && m_Content.activeInHierarchy;

        private void Awake()
        {
            if (m_CloseButton != null) m_CloseButton.onClick.AddListener(HideInternal);
        }

        private void OnEnable()
        {
            if (UpdateBank.Instance != null) UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
            BindManagerInternal();
        }

        private void BindManagerInternal()
        {
            var player = RushPlayer.Instance;
            var manager = player != null ? player.DailyAdsBundleManager : null;
            if (manager == m_Manager) return;
            if (m_Manager != null) m_Manager.OnBundlesUpdate.RemoveListener(RefreshInternal);
            m_Manager = manager;
            if (m_Manager != null) m_Manager.OnBundlesUpdate.AddListener(RefreshInternal);
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (!IsActive) return;
            FitDialogInternal();
            BindManagerInternal();
            RefreshDayInternal();
        }

        public void Tick()
        {
            if (m_Manager == null) BindManagerInternal();
            if (m_DisplayedDate != DateTime.Today) RefreshDayInternal();
            else RefreshTimerInternal();
        }

        private void RefreshDayInternal()
        {
            if (m_Manager == null) return;
            m_DisplayedDate = DateTime.Today;
            m_Manager.Init();
            RefreshTimerInternal();
        }

        private void RefreshInternal(DailyAdsBundleConfig[] bundles)
        {
            if (m_BundleViews == null || !IsActive) return;
            int claimed = 0;
            int total = 0;
            for (int i = 0; i < m_BundleViews.Length; i++)
            {
                var view = m_BundleViews[i];
                if (view == null) continue;
                var bundle = bundles != null && i < bundles.Length ? bundles[i] : null;
                if (bundle == null || bundle.Reward == null || bundle.RewardAmount <= 0)
                {
                    view.Hide();
                    continue;
                }
                view.Init(bundle, i);
                if (!view.IsShown) view.Show();
                total++;
                if (m_Manager.IsBundleClaimed(i)) claimed++;
            }
            m_Header.SetProgress(claimed, total);
        }

        private void RefreshTimerInternal()
        {
            if (m_Manager == null || m_ResetTimerText == null) return;
            int minutes = Math.Max(0, (int)Math.Ceiling(m_Manager.GetRemainingResetTime().TotalMinutes));
            if (minutes == m_LastMinutes) return;
            m_LastMinutes = minutes;
            var remaining = TimeSpan.FromMinutes(minutes);
            m_ResetTimerText.text = $"{(int)remaining.TotalHours:D2}h {remaining.Minutes:D2}m";
        }

        private void OnRectTransformDimensionsChange() => FitDialogInternal();

        private void FitDialogInternal()
        {
            if (m_DialogFit == null || !(m_DialogFit.parent is RectTransform stage)) return;
            float scale = Mathf.Min(1f, Mathf.Min(stage.rect.width / c_LayoutWidth, stage.rect.height / c_LayoutHeight));
            if (scale > 0f) m_DialogFit.localScale = new Vector3(scale, scale, 1f);
        }

        private void OnDisable()
        {
            if (UpdateBank.Instance != null) UpdateBank.Instance.UnregisterUpdateTick(gameObject);
            if (m_Manager != null) m_Manager.OnBundlesUpdate.RemoveListener(RefreshInternal);
            m_Manager = null;
            m_DisplayedDate = default;
        }

        private void OnDestroy()
        {
            if (m_CloseButton != null) m_CloseButton.onClick.RemoveListener(HideInternal);
        }
    }
}
