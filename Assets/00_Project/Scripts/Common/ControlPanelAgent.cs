using UnityEngine;

namespace LegionKnight
{
    public partial class ControlPanelAgent : MonoBehaviour
    {
        public void Show()
        {
            GameManager.Instance.ShowPanel(PanelId.ControlPanelId);
        }
        public void Hide()
        {
            GameManager.Instance.HidePanel(PanelId.ControlPanelId);
        }

        private ControlPanel GetControlPanel()
        {
            return GameManager.Instance.GetPanel<ControlPanel>();
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
