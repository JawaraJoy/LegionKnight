using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class EnergyCost : MonoBehaviour
    {
        [SerializeField]
        private Energy[] m_Costs;

        [SerializeField]
        private UnityEvent<Energy[]> m_OnCanPay;
        [SerializeField]
        private UnityEvent<Energy[]> m_OnCantPay;

        private void Awake()
        {
            Player.Instance.AddOnCanPayEnergies(OnCanPayInvoke);
            Player.Instance.AddOnCantPayEnergies(OnCantPayInvoke);
        }

        private void TradeInternal()
        {
            Player.Instance.PayEnergies(m_Costs);
        }

        public void Trade()
        {
            TradeInternal();
        }

        private void OnCanPayInvoke(Energy[] costs) 
        {
            m_OnCanPay.Invoke(costs);
        }

        private void OnCantPayInvoke(Energy[] costRest)
        {
            m_OnCantPay.Invoke(costRest);
        }
    }
}
