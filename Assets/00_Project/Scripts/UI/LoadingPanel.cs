using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string LoadingPanelId = "Loading";
    }
    public partial class LoadingPanel : PanelView
    {
        public override string UniqueId => PanelId.LoadingPanelId;
    }
}
