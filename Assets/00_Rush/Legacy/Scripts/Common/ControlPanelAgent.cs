using UnityEngine;

namespace LegionKnight
{
    public partial class ControlPanelAgent : MonoBehaviour
    {
        public void Show()
        {
            CanvasManager.Instance.ShowPanel(PanelId.ControlPanelId);
        }
        public void Hide()
        {
            CanvasManager.Instance.HidePanel(PanelId.ControlPanelId);
        }

        private ControlPanel GetControlPanel()
        {
            return CanvasManager.Instance.GetPanel<ControlPanel>();
        }

        public void SetJumpForceValueText(float val)
        {
            GetControlPanel().SetJumpForceValueText(val);
        }
        public void SetFallSpeedValueText(float val)
        {
            GetControlPanel().SetFallSpeedValueText(val);
        }
        public void SetMaxJumpDistance(float val)
        {
            GetControlPanel().SetMaxJumpDistance(val);
        }
    }

}
