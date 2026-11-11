using Rush;
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
            InitInternal();
        }

        public void Init()
        {
            InitInternal();
        }

        protected void InitInternal()
        {
            foreach (CurrencyView view in m_CurrencieViews)
            {
                view.Init();
            }
        }
        
        private CurrencyView GetCurrencyView(ItemConfig itemConfig)
        {
            CurrencyView match = m_CurrencieViews.Find(x => x.ItemConfig == itemConfig);
            return match;
        }

        public void SetCurrencyView(Currency currency)
        {
            GetCurrencyView(currency.ItemConfig).SetAmount(currency.Amount);
        }
    }
}
