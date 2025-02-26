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
        protected override void OnShowInvoke()
        {
            base.OnShowInvoke();
            Player.Instance.SetPause(true);
        }
        protected override void HideInternal()
        {
            base.HideInternal();
            Player.Instance.SetPause(false);
        }
    }
}
