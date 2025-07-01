using UnityEngine;

namespace LegionKnight
{
    public class RechargePanelAgent : MonoBehaviour
    {
        public void ShowRechargePanel()
        {
            GameManager.Instance.ShowRechargePanel();
        }
    }
    public partial class GameManager
    {
        public void ShowRechargePanel()
        {
            GetPanelInternal<RechargePanel>().Show();
        }
    }
}
