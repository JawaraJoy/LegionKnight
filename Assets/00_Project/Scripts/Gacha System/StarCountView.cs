using UnityEngine;

namespace LegionKnight
{
    public partial class StarCountView : CurrencyView
    {
        protected override void SetViewInternal(Currency currency)
        {
            base.SetViewInternal(currency);

            m_AmountText.text = $"x{currency.Amount} are Converted";
        }
    }
}
