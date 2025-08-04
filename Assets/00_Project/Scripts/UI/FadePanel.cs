using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string FadePanelId = "FadePanel";
    }
    public class FadePanel : PanelView
    {
        public override string UniqueId => PanelId.FadePanelId;
    }
}
