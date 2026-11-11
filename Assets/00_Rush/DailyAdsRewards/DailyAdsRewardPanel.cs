using UnityEngine;
using LegionKnight;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class DailyAdsRewardPanel : PanelView, IUpdater
    {
        [SerializeField] private TextMeshProUGUI m_ResetTimerText;
        [SerializeField] private Button m_CloseButton;
        [SerializeField] private DailyAdsBundleView[] m_BundleViews;
        private DailyAdsBundleManager m_Manager;
        private System.DateTime m_DisplayedDate;

        public DailyAdsBundleView[] BundleViews => m_BundleViews;
        public bool IsActive => m_Content != null && m_Content.activeInHierarchy;

        private void Awake()
        {
            if (m_CloseButton != null) m_CloseButton.onClick.AddListener(HideInternal);
            BindManagerInternal();
        }

        private void OnEnable()
        {
            if (UpdateBank.Instance != null) UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
            BindManagerInternal();
        }

        private void Start()
        {
            BindManagerInternal();
            if (m_Manager != null) Refresh(m_Manager.SelectedBundles);
        }

        private void BindManagerInternal()
        {
            var player = RushPlayer.Instance;
            var manager = player != null ? player.DailyAdsBundleManager : null;
            if (manager == m_Manager) return;
            if (m_Manager != null) m_Manager.OnBundlesUpdate.RemoveListener(Refresh);
            m_Manager = manager;
            if (m_Manager != null) m_Manager.OnBundlesUpdate.AddListener(Refresh);
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            BindManagerInternal();
            if (m_Manager == null) return;
            m_DisplayedDate = System.DateTime.Now.Date;
            m_Manager.Init();
            Refresh(m_Manager.SelectedBundles);
            UpdateResetTimer();
        }

        public void Tick()
        {
            if (m_Manager != null && m_DisplayedDate != System.DateTime.Now.Date)
            {
                m_DisplayedDate = System.DateTime.Now.Date;
                m_Manager.Init();
            }
            UpdateResetTimer();
        }

        private void Refresh(DailyAdsBundleConfig[] bundles)
        {
            if (m_BundleViews == null) return;
            for (int i = 0; i < m_BundleViews.Length; i++)
            {
                var view = m_BundleViews[i];
                if (view == null) continue;
                if (bundles != null && i < bundles.Length && bundles[i] != null)
                {
                    view.Init(bundles[i], i);
                    view.Show();
                }
                else
                {
                    view.Hide();
                }
            }
        }

        private void UpdateResetTimer()
        {
            if (m_Manager == null || m_ResetTimerText == null) return;
            var remaining = m_Manager.GetRemainingResetTime();
            m_ResetTimerText.text = $"(Refresh in {Mathf.Max(0, (int)remaining.TotalHours)}h {Mathf.Max(0, remaining.Minutes)}m)";
        }

        private void OnDisable()
        {
            if (UpdateBank.Instance != null) UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        private void OnDestroy()
        {
            if (m_CloseButton != null) m_CloseButton.onClick.RemoveListener(HideInternal);
            if (m_Manager != null) m_Manager.OnBundlesUpdate.RemoveListener(Refresh);
        }
    }
}
