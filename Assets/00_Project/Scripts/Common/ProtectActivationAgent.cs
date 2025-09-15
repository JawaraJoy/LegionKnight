using UnityEngine;

namespace LegionKnight
{

    public partial class ProtectActivationAgent : MonoBehaviour
    {
        [SerializeField]
        private int m_Shield;
        [SerializeField]
        private int m_Barrier;

        [SerializeField]
        private bool m_ActiveOnStart = true;
        private void Start()
        {
            if (m_ActiveOnStart)
            {
                ActiveInternal();
            }
        }
        public void LoadProtectAsset()
        {
            Player.Instance.LoadProtectAsset();
            ActiveInternal();
        }
        private void ActiveInternal()
        {
            Player.Instance.AddBarrier(m_Barrier);
            Player.Instance.AddShield(m_Shield);
            Player.Instance.SetProtectActivationAgent(this);
        }
        public void Active()
        {
            ActiveInternal();
        }
        public void Active(int shield, int barrier)
        {
            m_Shield = shield;
            m_Barrier = barrier;
            Player.Instance.AddBarrier(m_Barrier);
            Player.Instance.AddShield(m_Shield);
            Player.Instance.SetProtectActivationAgent(this);
        }
    }
}
