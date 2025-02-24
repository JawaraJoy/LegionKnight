using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string HomePanelId = "Home";
    }
    public partial class HomePanel : PanelView
    {
        public override string UniqueId => PanelId.HomePanelId;
    }
}
