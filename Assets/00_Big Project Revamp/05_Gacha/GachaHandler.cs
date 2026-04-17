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

        [SerializeField] private UnityEvent<GachaDrawResult> m_OnDrawComplete;
        [SerializeField] private UnityEvent<string> m_OnDrawFailed;
        [SerializeField] private UnityEvent<GachaConfirmData> m_OnDrawRequested;

        private GachaBannerConfig m_ActiveBanner;

        public GachaBannerConfig ActiveBanner => m_ActiveBanner;
        public GachaBannerConfig[] Banners => m_Banners;
        public GachaPityTracker PityTracker => m_PityTracker;
        public UnityEvent<GachaDrawResult> OnDrawComplete => m_OnDrawComplete;
        public UnityEvent<string> OnDrawFailed => m_OnDrawFailed;
        public UnityEvent<GachaConfirmData> OnDrawRequested => m_OnDrawRequested;

        protected virtual void Awake()
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
            if (!breakdown.CanAfford) { m_OnDrawFailed?.Invoke("Mata uang tidak cukup."); return; }
            m_OnDrawRequested?.Invoke(new GachaConfirmData(m_ActiveBanner, breakdown, false));
        }

        public void RequestDrawMulti()
        {
            if (!ValidateBannerInternal()) return;
            var breakdown = BuildBreakdownInternal(true);
            if (!breakdown.CanAfford) { m_OnDrawFailed?.Invoke("Mata uang tidak cukup."); return; }
            m_OnDrawRequested?.Invoke(new GachaConfirmData(m_ActiveBanner, breakdown, true));
        }

        // ── Execute Draw (post-confirm) ───────────────────────────────────────

        public void ExecuteDrawSingle()
        {
            if (!ValidateBannerInternal()) return;
            bool isDaily = m_CostResolver.IsDailyFirstDraw(m_ActiveBanner, false);
            var breakdown = BuildBreakdownInternal(false);
            if (!breakdown.CanAfford) { m_OnDrawFailed?.Invoke("Mata uang tidak cukup."); return; }

            m_CostResolver.DeductCost(m_ActiveBanner, GetCurrencyInternal(), breakdown);
            if (isDaily) m_CostResolver.MarkDailyDrawUsed(m_ActiveBanner, false);
            m_OnDrawComplete?.Invoke(ExecuteDrawsInternal(1));
        }

        public void ExecuteDrawMulti()
        {
            if (!ValidateBannerInternal()) return;
            bool isDaily = m_CostResolver.IsDailyFirstDraw(m_ActiveBanner, true);
            var breakdown = BuildBreakdownInternal(true);
            if (!breakdown.CanAfford) { m_OnDrawFailed?.Invoke("Mata uang tidak cukup."); return; }

            m_CostResolver.DeductCost(m_ActiveBanner, GetCurrencyInternal(), breakdown);
            if (isDaily) m_CostResolver.MarkDailyDrawUsed(m_ActiveBanner, true);
            m_OnDrawComplete?.Invoke(ExecuteDrawsInternal(m_ActiveBanner.MultiDrawCount));
        }

        private GachaDrawResult ExecuteDrawsInternal(int count)
        {
            var result = new GachaDrawResult();
            for (int i = 0; i < count; i++)
            {
                m_PityTracker.IncrementDraw();
                var item = m_DrawResolver.Resolve(m_ActiveBanner, m_PityTracker);
                if (item == null) continue;

                result.AddItem(item);

                // Pity triggered jika draw ini tepat menyentuh threshold
                // Resolver sudah reset counter setelah trigger, jadi cek sebelum increment
                // sudah dilakukan di dalam Resolve — flag di-set berdasarkan apakah
                // resolver memilih dari guarantee array
                result.SetPityTriggered(m_DrawResolver.LastDrawWasPity);

                m_CollectibleControl?.AddCollectible(item.Collect, item.Amount);
            }
            return result;
        }

        // ── Info ──────────────────────────────────────────────────────────────

        public List<GachaRateInfo> GetRateInfo(GachaBannerConfig banner)
        {
            var list = new List<GachaRateInfo>();
            if (banner?.Collectables == null) return list;

            float total = 0f;
            foreach (var c in banner.Collectables) total += c.Chance;
            if (total <= 0f) return list;

            foreach (var c in banner.Collectables)
                list.Add(new GachaRateInfo(c, c.Chance / total));
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
            if (m_ActiveBanner != null) return true;
            m_OnDrawFailed?.Invoke("Tidak ada banner aktif.");
            return false;
        }

        private CurrenciesControl GetCurrencyInternal() =>
            Player.Instance.CurrencyControl;    
    }
}