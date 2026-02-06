using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class SingleDrawButtonView : DrawButtonView
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
                Debug.LogWarning("GachaHandler not set on SingleDrawButtonView");
                return;
            }

            m_GachaHandler.PerformSingleDraw();
            
        }
    }
    public partial class BannerPanel
    {
        public void SetSingleDrawButton(GachaHandler handler, GachaCurrencyCost cost)
        {
            var view = GetBinding<SingleDrawButtonView>();
            view.SetButtonView(cost);   // existing UI method
            view.Init(handler);
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

            GachaCurrencyCost cost = ResolveSingleDrawCost(banner);
            GetBannerPanel().SetSingleDrawButton(m_GachaHandler, cost);
        }
        private GachaCurrencyCost ResolveSingleDrawCost(GachaBanner banner)
        {
            var def = banner.Definition;
            int drawCount = 1;

            var main = def.MainCurrency;
            int totalMainCost = main.Amount * drawCount;

            if (Player.Instance.GetCurrencyAmount(main.Definition) < totalMainCost)
            {
                var alt = def.AlternativeCurrency;
                return new GachaCurrencyCost(alt.Definition, alt.Amount * drawCount);
            }

            return new GachaCurrencyCost(main.Definition, totalMainCost);
        }

        
    }
}
