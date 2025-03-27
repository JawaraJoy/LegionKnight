using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string BannerPanelId = "Banner";
    }
    public partial class BannerPanel : PanelView
    {
        public override string UniqueId => PanelId.BannerPanelId;

    }
    public partial class GameManager
    {
        private BannerPanel GetBannerPanelInternal()
        {
            return GetPanel<BannerPanel>();
        }
    }
}
