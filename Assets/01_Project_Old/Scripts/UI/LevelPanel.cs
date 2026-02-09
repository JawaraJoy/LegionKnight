using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public const string LevelPanel = "LevelPanel";
    }
    public partial class LevelPanel : PanelView
    {
        public override string UniqueId => PanelId.LevelPanel;

    }
}
