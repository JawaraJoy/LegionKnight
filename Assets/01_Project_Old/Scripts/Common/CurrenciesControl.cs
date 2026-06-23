using MoreMountains.Tools;
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

        [Header("Sell Method")]
        [SerializeField, MMReadOnly]
        private CollectibleConfig m_SelectedSellConfig;
        [SerializeField, MMReadOnly]
        private int m_MaxSelectedSellAmount;
        [SerializeField, MMReadOnly]
        private int m_SelectedSellAmount;

        private UnityAction<int> m_OnSold;
        [SerializeField]
        private UnityEvent<CollectibleConfig, int, int> m_OnSelectedSellChanged;
        public UnityEvent<CollectibleConfig, int, int> OnSelectedSellChanged => m_OnSelectedSellChanged;

        public int SelectedSellAmount => m_SelectedSellAmount;
        public void Sell()
        {
            Currency sellValue = m_SelectedSellConfig.GetSellValue(m_SelectedSellAmount);

            CollectibleResultEntry entry = new CollectibleResultEntry(sellValue.ItemConfig, sellValue.Amount);
            CollectibleResultData resultData = new CollectibleResultData();
            resultData.AddEntry(entry.Collectible, entry.Amount);
            var resultPanel = CanvasManager.Instance.GetPanel<ShopResultPanel>();
            resultPanel.Show(resultData);
            //Player.Instance.CurrencyControl.AddCurrencyAmount(sellValue.ItemConfig, sellValue.Amount);
            CollectibleControl.AddCollectibleStatic("Preparation", sellValue.ItemConfig, sellValue.Amount);

            m_OnSold?.Invoke(m_SelectedSellAmount);
            SetSellTargetInternal(m_SelectedSellConfig, 0);
        }
        public void SetSellTarget(CollectibleConfig sellObject, int maxAmount, UnityAction<int> onSold)
        {
            SetSellTargetInternal(sellObject, maxAmount, onSold);
        }
        protected void SetSellTargetInternal(CollectibleConfig sellObject, int maxAmount, UnityAction<int> onSold = null)
        {
            m_OnSold = onSold;
            m_SelectedSellConfig = sellObject;
            m_MaxSelectedSellAmount = maxAmount;
            SetSelectedSellAmountInternal(1);
            OnSelectedSellChangedInvoke();
        }
        public void SetToMax()
        {
            m_SelectedSellAmount = m_MaxSelectedSellAmount;
            OnSelectedSellChangedInvoke();
        }
        public void AddSelectedSellAmount(int amount)
        {
            m_SelectedSellAmount += amount;
            OnSelectedSellChangedInvoke();
        }
        private void SetSelectedSellAmountInternal(int amount)
        {
            m_SelectedSellAmount = amount;
            OnSelectedSellChangedInvoke();
        }
        public void SetSelectedSellAmount(int amount)
        {
            SetSelectedSellAmountInternal(amount);
        }

        private void OnSelectedSellChangedInvoke()
        {
            m_SelectedSellAmount = Mathf.Clamp(m_SelectedSellAmount, 1, m_MaxSelectedSellAmount);
            m_OnSelectedSellChanged?.Invoke(m_SelectedSellConfig, m_SelectedSellAmount, m_MaxSelectedSellAmount);
        }
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
                    OnCurrencyChangeInvoke(currency);
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