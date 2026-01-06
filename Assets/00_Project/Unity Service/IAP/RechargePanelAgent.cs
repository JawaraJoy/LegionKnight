using UnityEngine;

namespace LegionKnight
{
    public class RechargePanelAgent : MonoBehaviour
    {
        public void ShowRechargePanel()
        {
            CanvasManager.Instance.ShowRechargePanel();
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
