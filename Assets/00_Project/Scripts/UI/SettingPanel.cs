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
            GameTimeScale.SetTimeScale(0);
            
        }
        protected override void HideInternal()
        {
            base.HideInternal();
            GameTimeScale.SetTimeScale(1);
        }
    }
}
