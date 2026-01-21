using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class AnimationGachaPanelAgent : MonoBehaviour
    {
        private AnimationGachaPanel m_Panel;
        private AnimationGachaPanel GetPanel()
        {
            if (m_Panel == null)
            {
                m_Panel = CanvasManager.Instance.GetPanel<AnimationGachaPanel>();
            }
            return m_Panel;
        }
        public void SpawnGacha(List<GachaReward> gacharRewards)
        {
            GetPanel().SpawnGacha(gacharRewards);
        }
    }
}
