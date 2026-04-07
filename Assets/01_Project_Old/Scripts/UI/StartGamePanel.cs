using Rush;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    
    public partial class StartGamePanel : PanelView
    {
        /*private bool m_Showed = false;

        private readonly string m_StartGameKey = "hasshowsg";
        protected override void OnShowInvoke()
        {
            base.OnShowInvoke();
            m_Showed = true;
            UnityService.Instance.SaveData(m_StartGameKey, true);
        }*/

        /*protected override void ShowInternal()
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
                RushGameManager.Instance.StageManager.PlayStage();
            }
            else
            {
                base.ShowInternal();
            }   
        }*/
    }
}
