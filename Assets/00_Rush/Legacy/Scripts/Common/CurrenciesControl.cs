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

        private PaidDiamondWallet m_PaidDiamonds;
        private Currency DiamondCurrency => m_Currencies.Find(c => PaidDiamondWallet.IsDiamond(c.ItemConfig));
        internal int LocalDiamonds
        {
            get
            {
                var c = DiamondCurrency ?? throw new System.InvalidOperationException("Diamond currency is missing.");
                var save = UnityService.Instance.LocalSave;
                string key = AmountKey(c.ItemConfig);
                return save.HasData(key) ? save.GetDataValue<int>(key) : c.Amount;
            }
        }

        internal void ShowPaidDiamonds(PaidDiamondWallet paid)
        {
            m_PaidDiamonds = paid;
            var currency = DiamondCurrency;
            if (currency != null) m_OnCurrencyChanged?.Invoke(currency);
        }

        internal void StageLocalDiamondBalance(int amount)
        {
            var currency = DiamondCurrency ?? throw new System.InvalidOperationException("Missing Diamond currency.");
            if (amount < 0) throw new System.InvalidOperationException("Insufficient local Diamonds.");
            var save = UnityService.Instance.LocalSave;
            save.SaveData(AmountKey(currency.ItemConfig), amount, callback: () =>
            {
                currency.SetAmount(amount);
                m_OnCurrencyChanged?.Invoke(currency);
            });
        }

        internal System.Threading.Tasks.Task CommitWithDiamondsAsync(int cost, System.Func<bool> valid, System.Action commit)
        {
            var save = UnityService.Instance.LocalSave;
            if (cost > 0 && m_PaidDiamonds != null)
                return m_PaidDiamonds.SpendAsync(this, save, cost, valid, commit);
            if (!valid()) throw new System.InvalidOperationException("Offer changed.");
            save.ExecuteTransaction(commit);
            return System.Threading.Tasks.Task.CompletedTask;
        }

        private bool m_IsSelling;
        public int SelectedSellAmount => m_SelectedSellAmount;
        private int SelectedSellMaximum => m_SelectedSellConfig != null && m_SelectedSellConfig.CanSell
            ? Mathf.Max(0, m_MaxSelectedSellAmount) : 0;

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
            UnityAction<int> onSold = m_OnSold;
            if (onSold == null) return;
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
                    onSold(amount);
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
            m_SelectedSellConfig = config != null && config.CanSell ? config : null;
            m_OnSold = m_SelectedSellConfig != null ? onSold : null;
            m_MaxSelectedSellAmount = m_SelectedSellConfig != null ? Mathf.Max(0, maxAmount) : 0;
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
            if (currency != null && m_PaidDiamonds != null && PaidDiamondWallet.IsDiamond(itemConfig))
                return (int)System.Math.Min(int.MaxValue, (long)LocalDiamonds + m_PaidDiamonds.Visible);
            return currency != null ? currency.Amount : 0;
        }
        public Currency GetCurrency(ItemConfig itemConfig)
        {
            return GetCurrencyInternal(itemConfig);
        }

        internal void StagePurchaseCredit(ItemConfig item, int amount)
        {
            var currency = GetCurrencyInternal(item);
            if (currency == null || amount < 1)
                throw new System.InvalidOperationException("The purchased currency is unavailable.");
            var save = UnityService.Instance.LocalSave;
            string key = AmountKey(item);
            int current = save.HasData(key) ? save.GetDataValue<int>(key) : currency.Amount;
            int total = checked(current + amount);
            save.SaveData(key, total, callback: () =>
            {
                currency.SetAmount(total);
                m_OnCurrencyChanged?.Invoke(currency);
            });
        }

        public bool TrySpendForHeroProgress(ItemConfig item, int cost, string progressKey, int next,
            System.Action applyProgress, System.Action notifyProgress)
        {
            if (item == null || cost <= 0 || !HasCurrency(item, out var balance) || balance.Amount < cost)
                return false;

            var save = UnityService.Instance.LocalSave;
            int remaining = balance.Amount - cost;
            save.ExecuteTransaction(() =>
            {
                save.SaveData(AmountKey(item), remaining);
                save.SaveData(progressKey, next);
            });

            applyProgress();
            balance.SetAmount(remaining);
            m_OnCurrencyChanged?.Invoke(balance);
            notifyProgress();
            return true;
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

            if (m_PaidDiamonds != null && PaidDiamondWallet.IsDiamond(itemConfig))
            {
                UnityService.Instance.LocalSave.ExecuteTransaction(() => StageLocalDiamondBalance(checked(LocalDiamonds + amount)));
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

            if (m_PaidDiamonds != null && PaidDiamondWallet.IsDiamond(itemConfig))
            {
                int local = m_PaidDiamonds.ApplyToDebit(amount);
                if (local > 0)
                    UnityService.Instance.LocalSave.ExecuteTransaction(() => StageLocalDiamondBalance(checked(LocalDiamonds - local)));
                return;
            }
            currency.RemoveAmount(amount);
            OnCurrencyChangeInvoke(currency);
        }
    }
}
