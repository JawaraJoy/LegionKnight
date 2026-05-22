using UnityEngine;
using LegionKnight;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class DailyAdsRewardPanel : PanelView, IUpdater
    {
        [SerializeField]
        private TextMeshProUGUI m_ResetTimerText;
        [SerializeField]
        private Button m_CloseButton;
        [SerializeField]
        private DailyAdsBundleView[] m_BundleViews;

        public DailyAdsBundleView[] BundleViews => m_BundleViews;

        public bool IsActive => gameObject.activeSelf;

        private void Start()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }

        public void Tick()
        {
            UpdateResetTimer();
        }

        private void Awake()
        {
            m_CloseButton.onClick.AddListener(HideInternal);
            RushPlayer.Instance.DailyAdsBundleManager.OnBundlesUpdate.AddListener(Refresh);
        }
        private void Refresh(DailyAdsBundleConfig[] bundles)
        {
            for (int i = 0; i < m_BundleViews.Length; i++)
            {
                if (i < bundles.Length)
                {
                    m_BundleViews[i].Show();
                    m_BundleViews[i].Init(bundles[i], i);
                }
                else
                {
                    m_BundleViews[i].Hide();
                }
            }
        }

        private void UpdateResetTimer()
        {
            var remaining =
                RushPlayer.Instance.DailyAdsBundleManager.GetRemainingResetTime();

            int hours = Mathf.Max(0, remaining.Hours);
            int minutes = Mathf.Max(0, remaining.Minutes);

            m_ResetTimerText.text =
                $"(Refresh in {hours}h {minutes}m)";
        }
    }
}
