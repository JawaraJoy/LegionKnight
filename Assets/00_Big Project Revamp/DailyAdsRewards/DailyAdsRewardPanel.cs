using UnityEngine;
using LegionKnight;
using UnityEngine.UI;

namespace Rush
{
    public class DailyAdsRewardPanel : PanelView
    {
        [SerializeField]
        private Button m_CloseButton;
        [SerializeField]
        private DailyAdsBundleView[] m_BundleViews;

        public DailyAdsBundleView[] BundleViews => m_BundleViews;

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
                    m_BundleViews[i].Init(bundles[i]);
                }
                else
                {
                    m_BundleViews[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
