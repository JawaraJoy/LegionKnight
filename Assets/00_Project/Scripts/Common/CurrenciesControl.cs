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
        private Currency GetCurrency(CurrencyDefinition definition)
        {
            Currency match = m_Currencies.Find(x => x.CurrencyDefinition == definition);
            return match;
        }
        public bool HasCurrency(CurrencyDefinition definition, out Currency currency)
        {
            bool has = GetCurrency(definition) != null;
            if (has)
            {
                currency = GetCurrency(definition);
            }
            else
            {
                currency = null;
            }
            return has;
        }
        public int GetCurrencyAmount(CurrencyDefinition definition)
        {
            return GetCurrency(definition).Amount;
        }
        public void SetCurrencyAmount(CurrencyDefinition definition, int amount)
        {
            GetCurrency(definition).SetAmount(amount);
        }
        public void AddCurrencyAmount(CurrencyDefinition definition, int amount)
        {
            GetCurrency(definition).AddAmount(amount);
        }
        public void RemoveCurrencyAmount(CurrencyDefinition definition, int amount)
        {
            GetCurrency(definition).RemoveAmount(amount);
        }
    }
}
