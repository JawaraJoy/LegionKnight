using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class EnergyCost : MonoBehaviour
    {
        [SerializeField]
        private Energy[] m_Costs;

        [SerializeField]
        private UnityEvent<Energy[], UnityAction<Energy[]>, UnityAction<Energy[]>> m_OnTryPay;
        [SerializeField]
        private UnityEvent<Energy[]> m_OnCanPay;
        [SerializeField]
        private UnityEvent<Energy[]> m_OnCantPay;

        private void PayInternal()
        {
            Player.Instance.EnergyController.Pay(m_Costs, OnCanPayInvoke, OnCantPayInvoke);
        }
        private void TryPayInternal()
        {
            Player.Instance.EnergyController.TryPay(m_Costs);
            OnTryPayInvoke(m_Costs);
        }

        public void Pay()
        {
            PayInternal();
        }
        public void TryPay()
        {
            TryPayInternal();
        }

        private void OnCanPayInvoke(Energy[] costs) 
        {
            m_OnCanPay.Invoke(costs);
            Player.Instance.EnergyController.OnCanPay.Invoke(costs);
        }

        private void OnCantPayInvoke(Energy[] costRest)
        {
            m_OnCantPay.Invoke(costRest);
            Player.Instance.EnergyController.OnCantPay.Invoke(costRest);
        }
        private void OnTryPayInvoke(Energy[] costs)
        {
            m_OnTryPay.Invoke(costs, OnCanPayInvoke, OnCantPayInvoke);
            Player.Instance.EnergyController.OnTryPay.Invoke(costs);
        }
    }
}
