using Rush;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class CurrenciesControl : MonoBehaviour
    {
        [SerializeField]
        protected List<Currency> m_Currencies = new();

        protected virtual void Start()
        {
            foreach(Currency currency in m_Currencies)
            {
                currency.Init();
            }
        }
        private Currency GetCurrency(ItemConfig config)
        {
            Currency match = m_Currencies.Find(x => x.ItemConfig.BaseInfo.Id == config.BaseInfo.Id);
            return match;
        }
        public bool HasCurrency(ItemConfig config, out Currency currency)
        {
            currency = GetCurrency(config);
            return currency != null;
        }
        public int GetCurrencyAmount(ItemConfig itemConfig)
        {
            return GetCurrency(itemConfig).Amount;
        }
        public void SetCurrencyAmount(ItemConfig itemConfig, int amount)
        {
            GetCurrency(itemConfig).SetAmount(amount);
        }
        public void AddCurrencyAmount(ItemConfig itemConfig, int amount)
        {
            GetCurrency(itemConfig).AddAmount(amount);
        }
        public void RemoveCurrencyAmount(ItemConfig itemConfig, int amount)
        {
            GetCurrency(itemConfig).RemoveAmount(amount);
        }
    }
}
