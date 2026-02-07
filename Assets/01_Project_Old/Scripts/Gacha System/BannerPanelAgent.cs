using UnityEngine;

namespace LegionKnight
{
    public partial class BannerPanelAgent : MonoBehaviour
    {
        private BannerPanel m_BannerPanel;
        private BannerPanel BannerPanel
        {
            get
            {
                if (m_BannerPanel == null)
                    m_BannerPanel = CanvasManager.Instance.GetPanel<BannerPanel>();
                return m_BannerPanel;
            }
        }
        private DrawCounterView m_DrawCounterView;

        public void SetDrawCountText(int currentDrawCount, int maxDrawCount)
        {
            if(m_DrawCounterView == null)
            {
                m_DrawCounterView = BannerPanel.GetBinding<DrawCounterView>();
            }
            m_DrawCounterView.SetCounterText(currentDrawCount, maxDrawCount);
        }
    }
}
