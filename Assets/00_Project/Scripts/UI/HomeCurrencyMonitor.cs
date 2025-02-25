using UnityEngine;

namespace LegionKnight
{
    public partial class HomeCurrencyMonitor : CurrencyMonitor
    {
        
    }

    public partial class HomePanel
    {
        private HomeCurrencyMonitor GetCurrencyMonitor()
        {
            return GetBinding<HomeCurrencyMonitor>();
        }

        public void SetCurrencyViewAmount(Currency currency)
        {
            GetCurrencyMonitor().SetCurrencyViewAmount(currency);
        }
    }

    public partial class GameManager
    {
        public void SetHomeCurrencyViewAmount(Currency currency)
        {
            GetPanelInternal<HomePanel>().SetCurrencyViewAmount(currency);
        }
    }
}
