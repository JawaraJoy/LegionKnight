using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class SingleDrawButtonView : DrawButtonView
    {
        private void Start()
        {
            UnityAction performDraw = new UnityAction(GameManager.Instance.PerformingSingleDraw);
            m_DrawButton.onClick.RemoveAllListeners();
            m_DrawButton.onClick.AddListener(() => OnButtonClick(performDraw));
            m_DrawButton.onClick.AddListener(() => OnButtonClickBanner(m_Cost));
        }
    }
    public partial class BannerPanel
    {
        public void SetSingleDrawButtonView(GachaCurrencyCost cost)
        {
            GetBinding<SingleDrawButtonView>().SetButtonView(cost);
        }
    }
    public partial class GachaManagerAgent
    {
        public void SetSingleDrawButtonView(GachaBanner banner)
        {
            BannerDefinition definition = banner.Definition;
            int mainCost = banner.GetFinalSingleDrawCost();
            int playerCurrencyAmount = Player.Instance.GetCurrencyAmount(definition.MainCurrencyToDraw.Definition);

            bool useAlternativeCurrency = playerCurrencyAmount < mainCost;
            GachaCurrencyCost cost = useAlternativeCurrency
                ? definition.AlternatifCurrencyToDraw
                : definition.MainCurrencyToDraw;

            GachaCurrencyCost singleDrawCost = new GachaCurrencyCost(cost.Definition, mainCost);
            GetBannerPanel().SetSingleDrawButtonView(cost);
        }
    }
}
