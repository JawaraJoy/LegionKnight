using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class MultiDrawButtonView : DrawButtonView
    {
        private GachaHandler m_GachaHandler;
        public void Init(GachaHandler handler)
        {
            m_GachaHandler = handler;

            m_DrawButton.onClick.RemoveAllListeners();
            m_DrawButton.onClick.AddListener(OnClick);
        }
        private void OnClick()
        {
            if (m_GachaHandler == null)
            {
                Debug.LogWarning("GachaHandler not set on MultiDrawButtonView");
                return;
            }
            m_GachaHandler.PerformMultiDraw();
        }
    }

    public partial class BannerPanel
    {
        public void SetMultiDrawButton(GachaHandler handler, GachaCurrencyCost cost)
        {
            var view = GetBinding<MultiDrawButtonView>();
            view.SetButtonView(cost);   // existing UI binding
            view.Init(handler);
        }
    }
    public partial class GachaManagerAgent
    {
        
        public void RefreshMultiDrawButton()
        {
            var banner = GachaHandler.GetSelectedBanner();
            if (banner == null)
                return;

            GachaCurrencyCost cost = ResolveMultiDrawCost(banner);
            GetBannerPanel().SetMultiDrawButton(GachaHandler, cost);
        }
        private GachaCurrencyCost ResolveMultiDrawCost(GachaBanner banner)
        {
            var def = banner.Definition;
            int drawCount = banner.Definition.MultiDraw; // assuming multi-draw is 10 draws

            var main = def.MainCurrency;
            int totalMainCost = main.Amount * drawCount;

            if (Player.Instance.GetCurrencyAmount(main.Definition) < totalMainCost)
            {
                var alt = def.AlternativeCurrency;
                return new GachaCurrencyCost(alt.Definition, totalMainCost);
            }
            return new GachaCurrencyCost(main.Definition, totalMainCost);

        }
    }
}
