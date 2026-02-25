using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class MultiDrawButtonView : DrawButtonView
    {
        
        public void PerFormMultiDraw()
        {
            GachaHandler.PerformMultiDraw();
        }
    }

    public partial class BannerPanel
    {
        public void SetMultiDrawButton(GachaCurrencyCost cost, int originalAmount)
        {
            var view = GetBinding<MultiDrawButtonView>();
            view.SetButtonView(cost, originalAmount);   // existing UI binding
            //view.Init(handler);
        }
    }
    public partial class GachaManagerAgent
    {
        
        public void RefreshMultiDrawButton()
        {
            var banner = GachaHandler.GetSelectedBanner();
            if (banner == null)
                return;

            int drawCount = banner.Definition.MultiDraw;

            // Tentukan currency yang dipakai (main / alt)
            var resolvedCost = ResolveCost(banner, drawCount);

            int originalAmount = banner.GetBaseCostForCurrency(
                resolvedCost.ItemConfig,
                drawCount
            );

            GetBannerPanel().SetMultiDrawButton(resolvedCost, originalAmount);
        }
        private GachaCurrencyCost ResolveCost(GachaBanner banner, int drawCount)
        {
            var main = banner.Definition.MainCurrency;
            int mainCost = banner.GetFinalCurrencyCost(main.ItemConfig, drawCount).Amount;

            if (Player.Instance.CurrencyControl.GetCurrencyAmount(main.ItemConfig) >= mainCost)
                return new GachaCurrencyCost(main.ItemConfig, mainCost);

            var alt = banner.Definition.AlternativeCurrency;
            int altCost = banner.GetFinalCurrencyCost(alt.ItemConfig, drawCount).Amount;

            return new GachaCurrencyCost(alt.ItemConfig, altCost);
        }
    }
}
