using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class GachaHandler : MonoBehaviour
    {
        [SerializeField]
        private List<GachaBanner> m_Banners = new();
        [SerializeField]
        private UnityEvent<GachaBanner> m_OnPerformDraw = new();
        [SerializeField]
        private UnityEvent<GachaBanner> m_OnBannerSelected = new();
        [SerializeField]
        private UnityEvent<GachaCurrencyCost> m_OnPerformDrawCost = new();

        [SerializeField]
        private GachaBanner m_SelectedBanner;
        [SerializeField]
        private UnityEvent<GachaBanner> m_OnStart = new();
        [SerializeField]
        private UnityEvent<Currency> m_OnConvertToStarCountChanged = new();

        [SerializeField]
        private Currency m_StarConvertCount;
        private void Start()
        {
            m_SelectedBanner = m_Banners[0];
            OnStartInvoke();
        }
        public void AddStarConvertCount(int amount)
        {
            m_StarConvertCount.AddAmount(amount);
            OnConvertToStarCountChangedInvoke(m_StarConvertCount);
            Player.Instance.AddCurrencyAmount(m_StarConvertCount.CurrencyDefinition, amount);
        }
        private void OnConvertToStarCountChangedInvoke(Currency currency)
        {
            m_StarConvertCount = currency;
            m_OnConvertToStarCountChanged?.Invoke(currency);
        }
        private void OnStartInvoke()
        {
            m_OnStart?.Invoke(m_SelectedBanner);
        }
        private GachaBanner GetGachaBannerInternal(BannerDefinition definition)
        {
            GachaBanner match = m_Banners.Find(x => x.Definition == definition);
            return match;
        }
        public void SelectBanner(BannerDefinition definition)
        {
            m_SelectedBanner = GetGachaBannerInternal(definition);
            OnBannerSelected(m_SelectedBanner);

        }
        public void PerformingSingleDraw()
        {
            m_StarConvertCount.SetAmount(0);
            m_SelectedBanner.PerformingSingleDraw();
            OnPerformDrawInvoke(m_SelectedBanner);
            OnPerformDrawCost(m_SelectedBanner.GetFinalCurrencyCost(1));
            OnStartInvoke();
        }

        public void PerformingMultiDraw()
        {
            m_StarConvertCount.SetAmount(0);
            m_SelectedBanner.PerformingMultiDraw();
            OnPerformDrawInvoke(m_SelectedBanner);
            OnPerformDrawCost(m_SelectedBanner.GetFinalCurrencyCost(m_SelectedBanner.MultiDraw));
            OnStartInvoke();
        }

        private void OnPerformDrawInvoke(GachaBanner banner)
        {
            m_OnPerformDraw?.Invoke(banner);
        }
        private void OnBannerSelected(GachaBanner banner)
        {
            m_OnBannerSelected?.Invoke(banner);
        }
        private void OnPerformDrawCost(GachaCurrencyCost cost)
        {
            m_OnPerformDrawCost?.Invoke(cost);
        }
    }
}
