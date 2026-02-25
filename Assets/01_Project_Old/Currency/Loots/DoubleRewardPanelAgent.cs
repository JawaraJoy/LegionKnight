using Rush;
using UnityEngine;

namespace LegionKnight
{
    public class DoubleRewardPanelAgent : MonoBehaviour
    {
        private DoubleRewardPanel m_DoubleRewardPanel;

        private DoubleRewardPanel DoubleRewardPanel
        {
            get
            {
                if (m_DoubleRewardPanel == null)
                {
                    m_DoubleRewardPanel = CanvasManager.Instance.GetPanel<DoubleRewardPanel>();
                }
                return m_DoubleRewardPanel;
            }
        }

        public void AddMainLoot(LootField loot)
        {
            DoubleRewardPanel.AddMainLoot(loot);
        }
        public void AddMirrorLoot(LootField loot)
        {
            DoubleRewardPanel.AddMirrorLoot(loot);
        }
        public void CopyMirrorFromLooted()
        {
            DoubleRewardPanel.CopyMirrorFromLooted();
        }
    }
}
