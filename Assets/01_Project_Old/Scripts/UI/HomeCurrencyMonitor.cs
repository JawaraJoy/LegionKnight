using Rush;
using UnityEngine;

namespace LegionKnight
{
    public partial class HomeCurrencyMonitor : CurrencyMonitor
    {
        private CurrenciesControl m_CurrencyControl;

        private CurrenciesControl CurrenciesControl
        {
            get
            {
                if (m_CurrencyControl == null)
                {
                    m_CurrencyControl = Player.Instance.CurrencyControl;
                }
                return m_CurrencyControl;
            }
        }
        private void Awake()
        {
            CurrenciesControl.OnCurrencyChanged.AddListener((_) => InitInternal());
        }
    }

    public partial class HomePanel
    {
        private HomeCurrencyMonitor GetCurrencyMonitor()
        {
            return GetBinding<HomeCurrencyMonitor>();
        }

        public void SetCurrencyViewAmount(Currency currency)
        {
            GetCurrencyMonitor().SetCurrencyView(currency);
        }
    }

    public partial class CanvasManager
    {
        public void SetHomeCurrencyView(Currency currency)
        {
            GetPanelInternal<HomePanel>().SetCurrencyViewAmount(currency);
        }
    }
}
