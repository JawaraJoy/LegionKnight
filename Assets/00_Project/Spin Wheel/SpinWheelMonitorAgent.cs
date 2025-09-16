using UnityEngine;

namespace LegionKnight
{
    public class SpinWheelMonitorAgent : MonoBehaviour
    {
        private SpinWheelMonitor m_Monitor;

        private SpinWheelPanel GetPanel()
        {
            return GameManager.Instance.GetPanel<SpinWheelPanel>();
        }
        private SpinWheelMonitor GetMonitor()
        {
            if (m_Monitor == null)
            {
                m_Monitor = GetPanel().GetBinding<SpinWheelMonitor>();
            }
            return m_Monitor;
        }
        public void SetSelected(SpinRewardDefinition selectedDefi)
        {
            GetMonitor().SetSelected(selectedDefi);
        }
        public void Claim()
        {
            GetMonitor().Claim();
        }
        public void BusyButtons(bool active)
        {
            GetMonitor().BusyButtons(active);
        }
    }
}
