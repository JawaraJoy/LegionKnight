using UnityEngine;
using UnityEngine.Events;
using LegionKnight;

namespace Rush
{
    public class ShopHandler : MonoBehaviour
    {
        [SerializeField] private ShopConfig m_ShopConfig;
        [SerializeField] private ShopCostResolver m_CostResolver;
        [SerializeField] private ShopPurchaseTracker m_PurchaseTracker;
        [SerializeField] private CollectibleControl m_CollectibleControl;

        [SerializeField] private UnityEvent<ShopConfirmData> m_OnPurchaseRequested;
        [SerializeField] private UnityEvent<CollectibleResultData> m_OnPurchaseComplete;
        [SerializeField] private UnityEvent<ShopBundleConfig> m_OnPurchaseCompleteBundle;
        [SerializeField] private UnityEvent<string> m_OnPurchaseFailed;

        private bool m_Committing;

        public ShopConfig ShopConfig => m_ShopConfig;
        public UnityEvent<ShopConfirmData> OnPurchaseRequested => m_OnPurchaseRequested;
        public UnityEvent<CollectibleResultData> OnPurchaseComplete => m_OnPurchaseComplete;
        public UnityEvent<ShopBundleConfig> OnPurchaseCompleteBundle => m_OnPurchaseCompleteBundle;
        public UnityEvent<string> OnPurchaseFailed => m_OnPurchaseFailed;

        public void RequestPurchase(ShopBundleConfig bundle)
        {
            if (!ValidateBundleInternal(bundle)) return;

            var availability = GetAvailability(bundle);
            if (!availability.CanPurchase)
            {
                OnPurchaseFailedInvoke("Bundle tidak tersedia saat ini.");
                return;
            }

            var breakdown = m_CostResolver.CalculateBreakdown(
                bundle, GetCurrencyInternal(), availability.IsFirstPurchase);

            if (!breakdown.CanAfford)
            {
                OnPurchaseFailedInvoke("Mata uang tidak cukup.");
                return;
            }

            OnPurchaseRequestedInvoke(new ShopConfirmData(bundle, breakdown));
        }

        public void ExecutePurchase(ShopBundleConfig bundle) => CommitInternal(bundle, null);

        public void ConfirmPurchase(ShopConfirmData offer)
        {
            if (offer == null || offer.Accepted) return;
            offer.MarkAccepted();
            CommitInternal(offer.Bundle, offer);
        }

        private async void CommitInternal(ShopBundleConfig bundle, ShopConfirmData offer)
        {
            if (m_Committing) return;
            if (!ValidateBundleInternal(bundle)) return;

            var availability = GetAvailability(bundle);
            if (!availability.CanPurchase)
            {
                OnPurchaseFailedInvoke("Bundle tidak tersedia saat ini.");
                return;
            }

            var breakdown = m_CostResolver.CalculateBreakdown(
                bundle, GetCurrencyInternal(), availability.IsFirstPurchase);

            if (!breakdown.CanAfford)
            {
                OnPurchaseFailedInvoke("Mata uang tidak cukup.");
                return;
            }

            if (offer != null && breakdown.MainCurrencyAmount != offer.MainCurrencyAmount)
            {
                OnPurchaseFailedInvoke("Harga telah berubah. Periksa kembali detail bundle.");
                return;
            }

            if (!FitsInventoryInternal(bundle, breakdown))
            {
                OnPurchaseFailedInvoke("Ruang inventori tidak cukup untuk seluruh isi bundle.");
                return;
            }

            var result = BuildResultInternal(bundle);
            m_Committing = true;
            try
            {
                var wallet = GetCurrencyInternal();
                int diamonds = PaidDiamondWallet.IsDiamond(bundle.CostCurrency) ? breakdown.MainCurrencyAmount : 0;
                bool StillValid()
                {
                    if (!this) return false;
                    var current = GetAvailability(bundle);
                    var price = m_CostResolver.CalculateBreakdown(bundle, wallet, current.IsFirstPurchase);
                    return current.CanPurchase && price.CanAfford && price.MainCurrencyAmount == breakdown.MainCurrencyAmount &&
                        FitsInventoryInternal(bundle, price);
                }
                await wallet.CommitWithDiamondsAsync(diamonds, StillValid, () =>
                {
                    m_CostResolver.DeductCost(bundle, GetCurrencyInternal(), breakdown);
                    result.GrantRewards(bundle.BaseInfo.Id);
                    m_PurchaseTracker.MarkPurchased(bundle);
                });
                OnPurchaseCompleteInvoke(result);
                OnPurchaseCompleteBundleInvoke(bundle);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("[Shop] Purchase could not complete: " + ex.GetType().Name);
                OnPurchaseFailedInvoke("Purchase unavailable. Check your balance and connection, then try again.");
            }
            finally
            {
                m_Committing = false;
            }
        }

        public ShopBundleAvailability GetAvailability(ShopBundleConfig bundle)
        {
            bool canPurchase = m_PurchaseTracker.CanPurchase(bundle);
            bool isFirst = m_PurchaseTracker.IsFirstPurchase(bundle);
            double resetSeconds = m_PurchaseTracker.GetDailyResetSecondsRemaining(bundle);
            return new ShopBundleAvailability(
                canPurchase, isFirst, resetSeconds, bundle.PurchaseLimit);
        }

        public ShopCostBreakdown GetBreakdown(ShopBundleConfig bundle)
        {
            bool isFirst = m_PurchaseTracker.IsFirstPurchase(bundle);
            return m_CostResolver.CalculateBreakdown(
                bundle, GetCurrencyInternal(), isFirst);
        }

        private CollectibleResultData BuildResultInternal(ShopBundleConfig bundle)
        {
            var result = new CollectibleResultData();
            if (bundle.Entries == null) return result;
            foreach (var entry in bundle.Entries)
                result.AddEntry(entry.Collectible, entry.Amount);
            return result;
        }

        private bool ValidateBundleInternal(ShopBundleConfig bundle)
        {
            if (bundle?.BaseInfo != null && !string.IsNullOrWhiteSpace(bundle.BaseInfo.Id)
                && bundle.BasePrice >= 0 && bundle.Entries != null && bundle.Entries.Length > 0
                && System.Array.TrueForAll(bundle.Entries, CanDeliverInternal))
                return true;
            OnPurchaseFailedInvoke("Bundle tidak valid.");
            return false;
        }

        private bool CanDeliverInternal(ShopBundleEntry entry)
        {
            if (entry?.Collectible == null || entry.Amount <= 0) return false;
            var player = Player.Instance;
            return entry.Collectible switch
            {
                CardConfig card => player.PlayerCardDeck.GetCardOwned(card) != null,
                EnergyConfig energy => player.EnergyController.HasEnergy(energy),
                ItemConfig item => player.CurrencyControl.HasCurrency(item, out _),
                HeroUnitConfig hero => player.HeroesCollection.GetHeroUnit(hero) != null,
                _ => false
            };
        }

        private bool FitsInventoryInternal(ShopBundleConfig bundle, ShopCostBreakdown price)
        {
            var incoming = new System.Collections.Generic.Dictionary<CollectibleConfig, long>();
            var player = Player.Instance;
            foreach (var entry in bundle.Entries)
            {
                var reward = entry.Collectible;
                incoming.TryGetValue(reward, out long amount);
                amount += entry.Amount;
                incoming[reward] = amount;
                long current;
                int limit = int.MaxValue;
                switch (reward)
                {
                    case CardConfig card:
                        current = player.PlayerCardDeck.GetCardOwned(card).Amount;
                        break;
                    case ItemConfig currency:
                        current = player.CurrencyControl.GetCurrencyAmount(currency);
                        if (currency == bundle.CostCurrency) current -= price.MainCurrencyAmount;
                        break;
                    case EnergyConfig energy:
                        current = player.EnergyController.GetEnergy(energy).Amount;
                        if (!energy.CanBreakMaxAmount) limit = energy.MaxAmount;
                        break;
                    default:
                        continue;
                }
                if (current + amount > limit) return false;
            }
            return true;
        }

        private CurrenciesControl GetCurrencyInternal() =>
            Player.Instance.CurrencyControl;
        private void OnPurchaseRequestedInvoke(ShopConfirmData value0)
        {
            m_OnPurchaseRequested?.Invoke(value0);
        }

        private void OnPurchaseCompleteInvoke(CollectibleResultData value0)
        {
            m_OnPurchaseComplete?.Invoke(value0);
        }

        private void OnPurchaseCompleteBundleInvoke(ShopBundleConfig value0)
        {
            m_OnPurchaseCompleteBundle?.Invoke(value0);
        }

        private void OnPurchaseFailedInvoke(string value0)
        {
            m_OnPurchaseFailed?.Invoke(value0);
        }
    }
}

