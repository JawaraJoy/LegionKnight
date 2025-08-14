using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class PlayerEnergy : EnergyController
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerEnergy m_EnergyController;
        public void AddEnergy(EnergyDefinition definition, int amount)
        {
            m_EnergyController.Add(definition, amount);
        }
        public void SetEnergy(EnergyDefinition definition, int amount)
        {
            m_EnergyController.Set(definition, amount);
        }
        public void PayEnergies(Energy[] energyCosts)
        {
            m_EnergyController.Pay(energyCosts);
        }
        public void AddOnCanPayEnergies(UnityAction<Energy[]> action)
        {
            m_EnergyController.AddOnCanPay(action);
        }
        public void AddOnCantPayEnergies(UnityAction<Energy[]> action)
        {
            m_EnergyController.AddOnCantPay(action);
        }
    }
}
