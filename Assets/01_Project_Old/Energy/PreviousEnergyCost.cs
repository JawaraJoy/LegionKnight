using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class PreviousEnergyCost : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<Energy[], UnityAction<Energy[]>, UnityAction<Energy[]>> m_OnTryPay;
        [SerializeField]
        private UnityEvent<Energy[]> m_OnCanPay;
        [SerializeField]
        private UnityEvent<Energy[]> m_OnCantPay;

        [SerializeField]
        private UnityEvent m_OnNotPay;
        private void PayInternal()
        {
            Player.Instance.EnergyController.PayPreviouesCost(OnCanPayInvoke, OnCantPayInvoke);
        }
        private void TryPayInternal()
        {
            OnTryPayInvoke(Player.Instance.EnergyController.PreviousCost);
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
            if (costs == null || costs.Length == 0)
            {
                m_OnNotPay?.Invoke();   
            }
            else
            {
                Player.Instance.EnergyController.TryPayPreviousCost();
                m_OnTryPay.Invoke(costs, OnCanPayInvoke, OnCantPayInvoke);
                Player.Instance.EnergyController.OnTryPay.Invoke(costs);
            }
                
        }
    }
}
