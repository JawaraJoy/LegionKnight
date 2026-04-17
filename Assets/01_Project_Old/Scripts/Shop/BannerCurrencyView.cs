using UnityEngine;

namespace LegionKnight
{
    public partial class BannerCurrencyView : CurrencyView
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
            CurrenciesControl.OnCurrencyChanged.AddListener((_)=> InitInternal());
        }
        private void OnEnable()
        {
            SetViewInternal(CurrenciesControl.GetCurrency(m_ItemConfig));
        }
    }
}
