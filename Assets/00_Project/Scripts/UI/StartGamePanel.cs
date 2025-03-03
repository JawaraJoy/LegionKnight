using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string StartGamePanel = "StartGame";
    }
    public partial class StartGamePanel : PanelView
    {
        public override string UniqueId => PanelId.StartGamePanel;

        protected override void OnShowInvoke()
        {
            base.OnShowInvoke();
            Player.Instance.SetPause(true);
        }
        protected override void OnHideInvoke()
        {
            base.OnHideInvoke();
            //GameTimeScale.SetTimeScale(1);
            Player.Instance.SetPause(false);
        }
    }
}
