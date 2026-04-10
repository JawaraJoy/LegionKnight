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
        public PlayerEnergy EnergyController => m_EnergyController;
        public Energy[] PreviousEnergyCost => m_EnergyController.PreviousCost;
        public Energy GetEnergy(EnergyConfig definition)
        {
            return m_EnergyController.GetEnergy(definition);
        }
        public void AddEnergy(EnergyConfig definition, int amount)
        {
            m_EnergyController.Add(definition, amount);
        }
        public void SetEnergy(EnergyConfig definition, int amount)
        {
            m_EnergyController.Set(definition, amount);
        }
        public void PayEnergies(Energy[] energyCosts, UnityAction<Energy[]> onCanPayListen, UnityAction<Energy[]> onCantPayListen)
        {
            m_EnergyController.Pay(energyCosts, onCanPayListen, onCantPayListen);
        }
        public void TryPayEnergies(Energy[] energiyCosts)
        {
            m_EnergyController.TryPay(energiyCosts);
        }
        public void ClearPreviousEnergyCost()
        {
            m_EnergyController.ClearPreviousCost();
        }
        public void TryPayPreviousEnergyCost()
        {
            m_EnergyController.TryPayPreviousCost();
        }
        public void PayPreviouesEnergyCost(UnityAction<Energy[]> onCanPayListen, UnityAction<Energy[]> onCantPayListen)
        {
            m_EnergyController.PayPreviouesCost(onCanPayListen, onCantPayListen);
        }
        public UnityEvent<Energy[]> OnTryPayEnergy => m_EnergyController.OnTryPay;
        public UnityEvent<Energy[]> OnCanPayEnergy => m_EnergyController.OnCanPay;
        public UnityEvent<Energy[]> OnCantPayEnergy => m_EnergyController.OnCantPay;
    }
}
