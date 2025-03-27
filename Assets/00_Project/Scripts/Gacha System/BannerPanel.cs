using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string BannerPanelId = "Banner";
    }
    public partial class BannerPanel : PanelView
    {
        public override string UniqueId => PanelId.BannerPanelId;

        [SerializeField]
        private GachaBanner m_SelectedBanner;
        [SerializeField]
        private UnityEvent<GachaBanner> m_OnSetSelectedBanner = new();
        public void SetSeletedBanner(GachaBanner set)
        {
            m_SelectedBanner = set;
            OnSelectedBannerInvoke();
        }   
        private void OnSelectedBannerInvoke()
        {
            m_OnSetSelectedBanner?.Invoke(m_SelectedBanner); 
        }
    }
    public partial class GameManager
    {
        private BannerPanel GetBannerPanelInternal()
        {
            return GetPanel<BannerPanel>();
        }
    }
}
