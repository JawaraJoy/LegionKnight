using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string PausePanelId = "Pause";
    }
    public partial class PausePanel : PanelView
    {
        public override string UniqueId => PanelId.PausePanelId;

        protected override void HideInternal()
        {
            base.HideInternal();
            //Player.Instance.SetPause(false);
            GameTimeScale.SetTimeScale(1f);
            
        }
        protected override void ShowInternal()
        {
            base.ShowInternal();
            //Player.Instance.SetPause(true);
            GameTimeScale.SetTimeScale(0f);
        }

        public void DeathCoroutineEnable(bool set)
        {
            DeathCoroutine.SetIsEnable(true);
        }
    }
}
