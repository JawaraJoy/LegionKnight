using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string SettingPanelId = "Setting";
    }
    public partial class SettingPanel : PanelView
    {
        public override string UniqueId => PanelId.SettingPanelId;
    }
}
