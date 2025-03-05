using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string ControlPanelId = "Control";
    }
    public partial class ControlPanel : PanelView
    {
        public override string UniqueId => PanelId.ControlPanelId;
        [SerializeField]
        private TextMeshProUGUI m_JumpForceValueText;
        [SerializeField]
        private TextMeshProUGUI m_FallSpeedValueText;
        [SerializeField]
        private TextMeshProUGUI m_MaxJumpDistance;

        public void SetJumpForceValueText(float val)
        {
            m_JumpForceValueText.text = val.ToString();
        }
        public void SetFallSpeedValueText(float val)
        {
            m_FallSpeedValueText.text = val.ToString();
        }
        public void SetMaxJumpDistance(float val)
        {
            m_MaxJumpDistance.text = val.ToString();
        }
    }
}
