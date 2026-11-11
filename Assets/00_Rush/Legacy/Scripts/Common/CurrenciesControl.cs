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

        private bool m_IsSelling;
        public int SelectedSellAmount => m_SelectedSellAmount;
        private int SelectedSellMaximum
        {
            get
            {
                int maximum = Mathf.Max(0, m_MaxSelectedSellAmount);
                if (m_SelectedSellConfig is CardConfig cardConfig)
                {
                    CardUnit card = Player.Instance.PlayerCardDeck.GetCardOwned(cardConfig);
                    maximum = Mathf.Min(maximum, card?.Amount ?? 0);
                }
                return Mathf.Max(0, maximum);
            }
        }

        public void Sell() => SellInternal();
        private void SellInternal()
        {
            if (m_IsSelling) return;
            if (m_SelectedSellConfig == null ||
                m_SelectedSellAmount <= 0 || m_SelectedSellAmount > SelectedSellMaximum)
            {
                RefreshSelectedSellInternal();
                return;
            }

            CollectibleConfig config = m_SelectedSellConfig;
            int amount = m_SelectedSellAmount;
            CardConfig cardConfig = config as CardConfig;
            UnityAction<int> onSold = m_OnSold;
            if (cardConfig == null && onSold == null) return;
            Currency value = config.GetSellValue(amount);
            if (value.ItemConfig == null || value.Amount <= 0 || GetCurrencyInternal(value.ItemConfig) == null)
                return;

            var result = new CollectibleResultData();
            result.AddEntry(value.ItemConfig, value.Amount);
            m_IsSelling = true;
            try
            {
                UnityService.Instance.LocalSave.ExecuteTransaction(() =>
                {
                    // Card inventory is authoritative; do not trust a stale UI callback.
                    if (cardConfig != null)
                    {
                        CardUnit card = Player.Instance.PlayerCardDeck.GetCardOwned(cardConfig);
                        if (card == null || card.Amount < amount)
                            throw new System.InvalidOperationException("Insufficient card stock.");
                        Player.Instance.PlayerCardDeck.AddCardAmount(cardConfig, -amount);
                    }
                    else
                    {
                        onSold(amount);
                    }
                    result.GrantRewards("Preparation");
                });
                SetSellTargetInternal(config, 0);
            }
            finally
            {
                m_IsSelling = false;
            }
            CanvasManager.Instance.GetPanel<ShopResultPanel>()?.Show(result);
        }

        public void SetSellTarget(CollectibleConfig config, int maxAmount, UnityAction<int> onSold)
            => SetSellTargetInternal(config, maxAmount, onSold);
        protected void SetSellTargetInternal(CollectibleConfig config, int maxAmount, UnityAction<int> onSold = null)
        {
            m_OnSold = onSold;
            m_SelectedSellConfig = config;
            m_MaxSelectedSellAmount = Mathf.Max(0, maxAmount);
            SetSelectedSellAmountInternal(SelectedSellMaximum > 0 ? 1 : 0);
        }
        public void SetToMax() => SetSelectedSellAmountInternal(SelectedSellMaximum);
        public void AddSelectedSellAmount(int amount)
            => SetSelectedSellAmountInternal((int)System.Math.Min(int.MaxValue, System.Math.Max(0L, (long)m_SelectedSellAmount + amount)));
        public void SetSelectedSellAmount(int amount) => SetSelectedSellAmountInternal(amount);
        private void SetSelectedSellAmountInternal(int amount)
        {
            m_SelectedSellAmount = amount;
            RefreshSelectedSellInternal();
        }
        private void RefreshSelectedSellInternal()
        {
            int maximum = SelectedSellMaximum;
            m_SelectedSellAmount = maximum > 0 ? Mathf.Clamp(m_SelectedSellAmount, 1, maximum) : 0;
            OnSelectedSellChangedInvoke(maximum);
        }
        private void OnSelectedSellChangedInvoke(int maximum)
            => m_OnSelectedSellChanged?.Invoke(m_SelectedSellConfig, m_SelectedSellAmount, maximum);

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
