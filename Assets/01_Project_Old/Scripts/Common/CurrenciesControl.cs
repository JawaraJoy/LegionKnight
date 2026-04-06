using Rush;
using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class CurrenciesControl : MonoBehaviour
    {
        [SerializeField]
        protected List<Currency> m_Currencies = new();

        protected virtual void Start()
        {
            foreach (Currency currency in m_Currencies)
            {
                currency.Init();
            }
        }

        private Currency GetCurrency(ItemConfig config)
        {
            return m_Currencies.Find(x => x.ItemConfig.BaseInfo.Id == config.BaseInfo.Id);
        }

        public bool HasCurrency(ItemConfig config, out Currency currency)
        {
            currency = GetCurrency(config);
            return currency != null;
        }

        public int GetCurrencyAmount(ItemConfig itemConfig)
        {
            var currency = GetCurrency(itemConfig);
            return currency != null ? currency.Amount : 0;
        }

        public void SetCurrencyAmount(ItemConfig itemConfig, int amount)
        {
            var currency = GetCurrency(itemConfig);
            if (currency == null)
            {
                Debug.LogError($"Currency not found: {itemConfig.name}");
                return;
            }

            currency.SetAmount(amount);
        }

        public void AddCurrencyAmount(ItemConfig itemConfig, int amount)
        {
            var currency = GetCurrency(itemConfig);
            if (currency == null)
            {
                Debug.LogError($"Currency not found: {itemConfig.name}");
                return;
            }

            currency.AddAmount(amount);
        }

        public void RemoveCurrencyAmount(ItemConfig itemConfig, int amount)
        {
            var currency = GetCurrency(itemConfig);
            if (currency == null)
            {
                Debug.LogError($"Currency not found: {itemConfig.name}");
                return;
            }

            currency.RemoveAmount(amount);
        }
    }
}