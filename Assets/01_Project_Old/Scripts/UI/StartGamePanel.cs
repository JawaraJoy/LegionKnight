using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string StartGamePanel = "StartGame";
    }
    public partial class StartGamePanel : PanelView
    {
        public override string UniqueId => PanelId.StartGamePanel;
        private bool m_Showed = false;

        private string m_StartGameKey = "hasshowsg";
        protected override void OnShowInvoke()
        {
            base.OnShowInvoke();
            //Player.Instance.SetPause(true);
            m_Showed = true;
            UnityService.Instance.SaveData(m_StartGameKey, true);
        }
        protected override void OnHideInvoke()
        {
            base.OnHideInvoke();
            //GameTimeScale.SetTimeScale(1);
            //Player.Instance.SetPause(false);
        }

        protected override void ShowInternal()
        {
            bool hasShowed = UnityService.Instance.HasData(m_StartGameKey);
            if (hasShowed)
            {
                m_Showed = UnityService.Instance.GetData<bool>(m_StartGameKey);
            }
            if (m_Showed)
            {
                //Player.Instance.SetPause(false);
                //GameManager.Instance.Play();
            }
            else
            {
                base.ShowInternal();
            }   
        }
    }
}
