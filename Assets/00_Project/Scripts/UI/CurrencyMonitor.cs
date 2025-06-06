using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class CurrencyMonitor : UIView
    {
        [SerializeField]
        private List<CurrencyView> m_CurrencieViews = new();

        private void Start()
        {
            foreach(CurrencyView view in m_CurrencieViews)
            {
                view.Init();
            }
        }
        private CurrencyView GetCurrencyView(CurrencyDefinition definition)
        {
            CurrencyView match = m_CurrencieViews.Find(x => x.CurrencyDefinition == definition);
            return match;
        }

        public void SetCurrencyViewAmount(Currency currency)
        {
            GetCurrencyView(currency.CurrencyDefinition).SetAmount(currency.Amount);
        }
    }
}
