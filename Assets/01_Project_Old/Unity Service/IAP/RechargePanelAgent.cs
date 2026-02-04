using UnityEngine;

namespace LegionKnight
{
    public class RechargePanelAgent : MonoBehaviour
    {
        public void ShowRechargePanel()
        {
            CanvasManager.Instance.ShowRechargePanel();
        }
        public void ShowTab(string tabName)
        {
            CanvasManager.Instance.ShowRechargePanel();
            RechargePanel panel = CanvasManager.Instance.GetPanel<RechargePanel>();
            panel.ShowBinding(tabName);
        }
    }
    public partial class CanvasManager
    {
        public void ShowRechargePanel()
        {
            GetPanelInternal<RechargePanel>().Show();
        }
    }
}
