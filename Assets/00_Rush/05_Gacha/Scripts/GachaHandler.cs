using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LegionKnight;

namespace Rush
{
    public class GachaHandler : MonoBehaviour
    {
        [SerializeField] private GachaBannerConfig[] m_Banners;
        [SerializeField] private GachaPityTracker m_PityTracker;
        [SerializeField] private GachaDrawResolver m_DrawResolver;
        [SerializeField] private GachaCostResolver m_CostResolver;
        [SerializeField] private CollectibleControl m_CollectibleControl;

        [SerializeField] private UnityEvent<CollectibleResultData> m_OnDrawComplete;
        [SerializeField] private UnityEvent<string> m_OnDrawFailed;
        [SerializeField] private UnityEvent<GachaConfirmData> m_OnDrawRequested;

        private bool m_Committing;
        private GachaBannerConfig m_ActiveBanner;

        public GachaBannerConfig ActiveBanner => m_ActiveBanner;
        public GachaBannerConfig[] Banners => m_Banners;
        public GachaPityTracker PityTracker => m_PityTracker;
        public UnityEvent<CollectibleResultData> OnDrawComplete => m_OnDrawComplete;
        public UnityEvent<string> OnDrawFailed => m_OnDrawFailed;
        public UnityEvent<GachaConfirmData> OnDrawRequested => m_OnDrawRequested;

        internal GachaBannerConfig ResolveResultBanner(string source)
        {
            foreach (var banner in m_Banners)
                if (banner != null && banner.BaseInfo.Id == source) return banner;
            return m_ActiveBanner;
        }

        public void Init()
        {
            if (m_Banners is { Length: > 0 })
                SelectBannerInternal(m_Banners[0]);
        }
        // ── Banner ────────────────────────────────────────────────────────────

        private void SelectBannerInternal(GachaBannerConfig banner)
        {
            m_ActiveBanner = banner;
            m_PityTracker.Init(banner);
        }

        public void SelectBanner(GachaBannerConfig banner)
        {
            if (banner == null) return;
            SelectBannerInternal(banner);
        }

        public void SelectBanner(int index)
        {
            if (m_Banners == null || index < 0 || index >= m_Banners.Length) return;
            SelectBannerInternal(m_Banners[index]);
        }

        // ── Request Draw (pre-confirm) ────────────────────────────────────────

        public void RequestDrawSingle()
        {
            if (!ValidateBannerInternal()) return;
            var breakdown = BuildBreakdownInternal(false);
            if (!breakdown.CanAfford) { OnDrawFailedInvoke("Not enough currency for this draw."); return; }
            OnDrawRequestedInvoke(new GachaConfirmData(m_ActiveBanner, breakdown, false));
        }

        public void RequestDrawMulti()
        {
            if (!ValidateBannerInternal()) return;
            var breakdown = BuildBreakdownInternal(true);
            if (!breakdown.CanAfford) { OnDrawFailedInvoke("Not enough currency for this draw."); return; }
            OnDrawRequestedInvoke(new GachaConfirmData(m_ActiveBanner, breakdown, true));
        }

        // ── Execute Draw (post-confirm) ───────────────────────────────────────

        public void ExecuteDrawSingle() => ExecuteOfferInternal(false, null);
        public void ExecuteDrawMulti() => ExecuteOfferInternal(true, null);

        public void ConfirmDraw(GachaConfirmData offer)
        {
            if (offer == null || offer.Accepted) return;
            offer.MarkAccepted();
            if (offer.Banner == m_ActiveBanner)
                ExecuteOfferInternal(offer.IsMulti, offer);
        }

        private async void ExecuteOfferInternal(bool multi, GachaConfirmData offer)
        {
            if (m_Committing || !ValidateBannerInternal()) return;
            bool isDaily = m_CostResolver.IsDailyFirstDraw(m_ActiveBanner, multi);
            var breakdown = m_CostResolver.CalculateBreakdown(m_ActiveBanner, GetCurrencyInternal(), multi, isDaily);
            if (!breakdown.CanAfford)
            {
                OnDrawFailedInvoke("Not enough currency for this draw.");
                return;
            }
            if (offer != null && (offer.MainCurrencyAmount != breakdown.MainDeductAmount ||
                                  offer.AltCurrencyAmount != breakdown.AltDeductAmount))
            {
                OnDrawRequestedInvoke(new GachaConfirmData(m_ActiveBanner, breakdown, multi, true));
                return;
            }

            var banner = m_ActiveBanner;
            var wallet = GetCurrencyInternal();
            int diamonds = (PaidDiamondWallet.IsDiamond(banner.DrawCostCurrency) ? breakdown.MainDeductAmount : 0) +
                (PaidDiamondWallet.IsDiamond(banner.AltCostCurrency) ? breakdown.AltDeductAmount : 0);
            bool StillValid()
            {
                if (!this || m_ActiveBanner != banner || isDaily != m_CostResolver.IsDailyFirstDraw(banner, multi)) return false;
                var price = m_CostResolver.CalculateBreakdown(banner, wallet, multi, isDaily);
                return price.CanAfford && price.MainDeductAmount == breakdown.MainDeductAmount &&
                    price.AltDeductAmount == breakdown.AltDeductAmount;
            }
            m_Committing = true;
            try
            {
                CollectibleResultData result = null;
                await wallet.CommitWithDiamondsAsync(diamonds, StillValid, () =>
                {
                    m_CostResolver.DeductCost(banner, wallet, breakdown);
                    if (isDaily) m_CostResolver.MarkDailyDrawUsed(banner, multi);
                    result = ExecuteDrawsInternal(multi ? banner.MultiDrawCount : 1);
                    result.GrantRewards(banner.BaseInfo.Id);
                });
                OnDrawCompleteInvoke(result);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("[Summon] Draw could not complete: " + ex.GetType().Name);
                OnDrawFailedInvoke("Draw unavailable. Check your balance and connection, then try again.");
            }
            finally { m_Committing = false; }
        }

        private CollectibleResultData ExecuteDrawsInternal(int count)
        {
            var result = new CollectibleResultData();
            for (int i = 0; i < count; i++)
            {
                m_PityTracker.IncrementDraw();
                var item = m_DrawResolver.Resolve(m_ActiveBanner, m_PityTracker);
                if (item == null) throw new System.InvalidOperationException("Gacha banner did not resolve a reward.");

                result.AddEntry(item.Collect, item.Amount);
                result.SetSpecialDrop(m_DrawResolver.LastDrawWasPity);
            }
            return result;
        }

        // ── Info ──────────────────────────────────────────────────────────────

        public List<GachaRateInfo> GetRateInfo(GachaBannerConfig banner)
        {
            var list = new List<GachaRateInfo>();
            if (banner?.Collectables == null) return list;

            float total = GachaRateInfo.TotalWeight(banner.Collectables, banner);
            foreach (var entry in banner.Collectables)
                if (GachaRateInfo.IsEligible(entry))
                    list.Add(new GachaRateInfo(entry, banner.NormalWeight(entry) / total));
            return list;
        }

        public GachaCostBreakdown GetBreakdown(bool isMulti) => BuildBreakdownInternal(isMulti);

        public bool CanAffordDraw(bool isMulti) => BuildBreakdownInternal(isMulti).CanAfford;

        // ── Helpers ───────────────────────────────────────────────────────────

        private GachaCostBreakdown BuildBreakdownInternal(bool isMulti)
        {
            bool isDaily = m_CostResolver.IsDailyFirstDraw(m_ActiveBanner, isMulti);
            return m_CostResolver.CalculateBreakdown(
                m_ActiveBanner, GetCurrencyInternal(), isMulti, isDaily);
        }

        private bool ValidateBannerInternal()
        {
            if (m_ActiveBanner != null)
            {
                if (GachaRateInfo.TotalWeight(m_ActiveBanner.Collectables, m_ActiveBanner) > 0f) return true;
                OnDrawFailedInvoke("There are no rewards available in this banner.");
                return false;
            }
            OnDrawFailedInvoke("There is no active banner.");
            return false;
        }

        private CurrenciesControl GetCurrencyInternal() =>
            Player.Instance.CurrencyControl;    
        private void OnDrawCompleteInvoke(CollectibleResultData value0)
        {
            m_OnDrawComplete?.Invoke(value0);
        }

        private void OnDrawFailedInvoke(string value0)
        {
            m_OnDrawFailed?.Invoke(value0);
        }

        private void OnDrawRequestedInvoke(GachaConfirmData value0)
        {
            m_OnDrawRequested?.Invoke(value0);
        }
    }
}


