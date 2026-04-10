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

        [SerializeField]
        private UnityEvent<Currency> m_OnCurrencyChanged;
        public UnityEvent<Currency> OnCurrencyChanged => m_OnCurrencyChanged;

        private string AmountKey(ItemConfig itemConfig)
        {
            string key = $"amount{itemConfig.BaseInfo.Id}";
            return key;
        }

        private void OnCurrencyChangeInvoke(Currency currency)
        {
            m_OnCurrencyChanged?.Invoke(currency);
            int amountToSave = currency.Amount;
            UnityService.Instance.SaveData(AmountKey(currency.ItemConfig), amountToSave);
        }

        protected virtual void InitInternal()
        {
            foreach (Currency currency in m_Currencies)
            {
                bool hasCurrencyData = UnityService.Instance.HasData(AmountKey(currency.ItemConfig));
                if (hasCurrencyData)
                {
                    ItemConfig itemConfig = currency.ItemConfig;
                    int amount = UnityService.Instance.GetData<int>(AmountKey(currency.ItemConfig));
                    SetCurrencyAmount(itemConfig, amount);
                }
            }
        }
        public void Init()
        {
            InitInternal();
        }

        private Currency GetCurrencyInternal(ItemConfig config)
        {
            return m_Currencies.Find(x => x.ItemConfig.BaseInfo.Id == config.BaseInfo.Id);
        }

        public bool HasCurrency(ItemConfig config, out Currency currency)
        {
            currency = GetCurrencyInternal(config);
            return currency != null;
        }

        public int GetCurrencyAmount(ItemConfig itemConfig)
        {
            var currency = GetCurrencyInternal(itemConfig);
            return currency != null ? currency.Amount : 0;
        }
        public Currency GetCurrency(ItemConfig itemConfig)
        {
            return GetCurrencyInternal(itemConfig);
        }

        public void SetCurrencyAmount(ItemConfig itemConfig, int amount)
        {
            var currency = GetCurrencyInternal(itemConfig);
            if (currency == null)
            {
                Debug.LogError($"Currency not found: {itemConfig.name}");
                return;
            }

            currency.SetAmount(amount);
            OnCurrencyChangeInvoke(currency);
        }

        public void AddCurrencyAmount(ItemConfig itemConfig, int amount)
        {
            var currency = GetCurrencyInternal(itemConfig);
            if (currency == null)
            {
                Debug.LogError($"Currency not found: {itemConfig.name}");
                return;
            }

            currency.AddAmount(amount);
            OnCurrencyChangeInvoke(currency);
        }

        public void RemoveCurrencyAmount(ItemConfig itemConfig, int amount)
        {
            var currency = GetCurrencyInternal(itemConfig);
            if (currency == null)
            {
                Debug.LogError($"Currency not found: {itemConfig.name}");
                return;
            }

            currency.RemoveAmount(amount);
            OnCurrencyChangeInvoke(currency);
        }
    }
}