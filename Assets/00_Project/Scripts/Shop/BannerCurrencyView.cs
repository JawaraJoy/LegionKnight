using UnityEngine;

namespace LegionKnight
{
    public partial class BannerCurrencyView : CurrencyView
    {
        
        public void SelectBanner(ShopContainer container)
        {
            SetViewInternal(container.GetCurrencyUsed());
        }

        public void AdjustCurrency()
        {
            int playerCurrencyAmount = Player.Instance.GetCurrencyAmount(m_CurrencyDefinition);
            m_AmountText.text = playerCurrencyAmount.ToString();
        }
    }
}
