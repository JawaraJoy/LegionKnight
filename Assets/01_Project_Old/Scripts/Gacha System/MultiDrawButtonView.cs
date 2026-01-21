using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class MultiDrawButtonView : DrawButtonView
    {
        private void Start()
        {
            UnityAction performDraw = new UnityAction(GameManager.Instance.PerformingMultiDraw);
            m_DrawButton.onClick.RemoveAllListeners();
            m_DrawButton.onClick.AddListener(() => OnButtonClickBanner(m_Cost));
            m_DrawButton.onClick.AddListener(() => OnButtonClick(performDraw));
        }
    }

    public partial class BannerPanel
    {
        public void SetMultiDrawButtonView(GachaCurrencyCost banner)
        {
            GetBinding<MultiDrawButtonView>().SetButtonView(banner);
        }
    }
    public partial class GachaManagerAgent
    {
        public void SetMultieDrawButtonView(GachaBanner banner)
        {
            BannerDefinition definition = banner.Definition;
            int mainCost = banner.GetFinalMultiDrawCost();
            int playerCurrencyAmount = Player.Instance.GetCurrencyAmount(definition.MainCurrencyToDraw.Definition);

            bool useAlternativeCurrency = playerCurrencyAmount < mainCost;
            GachaCurrencyCost cost = useAlternativeCurrency
                ? definition.AlternatifCurrencyToDraw
                : definition.MainCurrencyToDraw;

            GachaCurrencyCost multiDrawCost = new GachaCurrencyCost(cost.Definition, mainCost);
            GetBannerPanel().SetMultiDrawButtonView(multiDrawCost);
        }
    }
}
