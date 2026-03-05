using UnityEngine;

namespace LegionKnight
{
    
    public partial class SettingPanel : PanelView
    {
        protected override void OnShowInvoke()
        {
            base.OnShowInvoke();
            //GameTimeScale.SetTimeScale(0);
            
        }
        protected override void HideInternal()
        {
            base.HideInternal();
            //GameTimeScale.SetTimeScale(1);
        }
    }
}
