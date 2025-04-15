using UnityEngine;

namespace LegionKnight
{
    public class GachaCurrencyView : CurrencyView
    {
        public void SetCurrency(GachaBanner banner)
        {
            GachaCurrencyCost cost = banner.GetSelectedGachaCurrencyCost();
            SetViewInternal(new Currency(cost.Definition, Player.Instance.GetCurrencyAmount(cost.Definition)));
        }

        private void OnEnable()
        {
            SetViewInternal(new Currency(m_CurrencyDefinition, Player.Instance.GetCurrencyAmount(m_CurrencyDefinition)));
        }
    }
}
