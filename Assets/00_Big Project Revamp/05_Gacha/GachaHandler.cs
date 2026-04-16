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

        private GachaBannerConfig m_ActiveBanner;

        public GachaBannerConfig ActiveBanner => m_ActiveBanner;
        public GachaBannerConfig[] Banners => m_Banners;
        public GachaPityTracker PityTracker => m_PityTracker;
        public UnityEvent<GachaDrawResult> OnDrawComplete => m_OnDrawComplete;
        public UnityEvent<string> OnDrawFailed => m_OnDrawFailed;

        protected virtual void Awake()
        {
            if (m_Banners is { Length: > 0 })
                SelectBannerInternal(m_Banners[0]);
        }

        // ── Banner Selection ──────────────────────────────────────────────────
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

        // ── Draw ──────────────────────────────────────────────────────────────
        public void DrawSingle()
        {
            if (!ValidateBannerInternal()) return;

            var currency = GetCurrencyInternal();
            bool isDaily = m_CostResolver.IsDailyFirstDraw(m_ActiveBanner, false);

            if (!m_CostResolver.HasEnoughCurrency(m_ActiveBanner, currency, false, isDaily))
            {
                m_OnDrawFailed?.Invoke("Mata uang tidak cukup.");
                return;
            }

            m_CostResolver.DeductCost(m_ActiveBanner, currency, false, isDaily);
            if (isDaily) m_CostResolver.MarkDailyDrawUsed(m_ActiveBanner, false);

            m_OnDrawComplete?.Invoke(ExecuteDrawsInternal(1));
        }

        public void DrawMulti()
        {
            if (!ValidateBannerInternal()) return;

            var currency = GetCurrencyInternal();
            bool isDaily = m_CostResolver.IsDailyFirstDraw(m_ActiveBanner, true);

            if (!m_CostResolver.HasEnoughCurrency(m_ActiveBanner, currency, true, isDaily))
            {
                m_OnDrawFailed?.Invoke("Mata uang tidak cukup.");
                return;
            }

            m_CostResolver.DeductCost(m_ActiveBanner, currency, true, isDaily);
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
                result.SetPityTriggered(
                    m_PityTracker.IsInFinalPityWindow || m_PityTracker.IsInSmallPityWindow);

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

        public int GetDrawCost(bool isMulti)
        {
            if (m_ActiveBanner == null) return 0;
            bool isDaily = m_CostResolver.IsDailyFirstDraw(m_ActiveBanner, isMulti);
            return m_CostResolver.CalculateCost(m_ActiveBanner, isMulti, isDaily);
        }

        public bool CanAffordDraw(bool isMulti)
        {
            if (m_ActiveBanner == null) return false;
            bool isDaily = m_CostResolver.IsDailyFirstDraw(m_ActiveBanner, isMulti);
            return m_CostResolver.HasEnoughCurrency(m_ActiveBanner, GetCurrencyInternal(), isMulti, isDaily);
        }

        // ── Helpers ───────────────────────────────────────────────────────────
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