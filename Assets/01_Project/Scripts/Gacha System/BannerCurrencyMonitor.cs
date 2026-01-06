using UnityEngine;

namespace LegionKnight
{
    public partial class BannerCurrencyMonitor : CurrencyMonitor
    {
        
    }
    public partial class BannerPanel
    {
        public void SetCurrencyView(Currency currency)
        {
            GetBinding<BannerCurrencyMonitor>().SetCurrencyView(currency);
        }
    }
    public partial class GachaManagerAgent
    {
        public void SetCurrencyView(Currency currency)
        {
            GetBannerPanel().SetCurrencyView(currency);
        }
    }
}
