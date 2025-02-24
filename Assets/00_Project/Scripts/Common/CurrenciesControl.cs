using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class CurrenciesControl : MonoBehaviour
    {
        [SerializeField]
        private List<Currency> m_Currencies = new();

        private Currency GetCurrency(CurrencyDefinition definition)
        {
            Currency match = m_Currencies.Find(x => x.CurrencyDefinition == definition);
            return match;
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
