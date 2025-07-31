using UnityEngine;

namespace LegionKnight
{
    public class Background : View
    {
        [SerializeField]
        private LoopTrigger[] m_LoopTriggers;

        [SerializeField]
        private Transform m_LoopTriggersParent;

        private void Start()
        {
            m_LoopTriggersParent.DetachChildren();
        }
    }
}
