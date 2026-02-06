using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class SingleDrawButtonView : DrawButtonView
    {
        public void PerFormSingleDraw()
        {
            GachaHandler.PerformSingleDraw();
        }
    }
    public partial class BannerPanel
    {
        public void SetSingleDrawButton(GachaCurrencyCost finalCost,int originalAmount)
        {
            var view = GetBinding<SingleDrawButtonView>();
            view.SetButtonView(finalCost, originalAmount);
        }
    }
    public partial class GachaManagerAgent
    {
        private GachaHandler m_GachaHandler;

        private GachaHandler GachaHandler
        {
            get
            {
                if (m_GachaHandler == null)
                    m_GachaHandler = GameManager.Instance.GachaMananger;
                return m_GachaHandler;
            }
        }

        public void RefreshSingleDrawButton()
        {
            var banner = GachaHandler.GetSelectedBanner();
            if (banner == null)
                return;

            int drawCount = 1;

            // Tentukan currency yang dipakai (main / alt)
            var resolvedCost = ResolveCost(banner, drawCount);

            int originalAmount = banner.GetBaseCostForCurrency(
                resolvedCost.Definition,
                drawCount
            );

            GetBannerPanel().SetSingleDrawButton(resolvedCost, originalAmount);
        }
    }
}
