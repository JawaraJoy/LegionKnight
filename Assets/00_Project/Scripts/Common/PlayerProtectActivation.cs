using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerProtectActivation : ProtectActivation
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerProtectActivation m_ProtectActivation;

        public void AddShield(int addHealth)
        {
            m_ProtectActivation.AddShield(addHealth);
        }
        public void AddBarrier(int addBarrier)
        {
            m_ProtectActivation.AddBarrier(addBarrier);
        }
        public void SetBarrier(int barrier)
        {
            m_ProtectActivation.SetBarrier(barrier);
        }
        public void SetShield(int shield)
        {
            m_ProtectActivation.SetShield(shield);
        }
        public void SetProtectActivationAgent(ProtectActivationAgent protectActivationAgent)
        {
            m_ProtectActivation.SetProtectActivationAgent(protectActivationAgent);
        }
        public void LoadProtectAsset()
        {
            m_ProtectActivation.LoadProtectAsset();
        }
    }
}
