using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class CurrencyView : UIView
    {
        [SerializeField]
        private CurrencyDefinition m_CurrencyDefinition;
        [SerializeField]
        private Image m_Icon;
        [SerializeField]
        private TextMeshProUGUI m_AmountText;
        public CurrencyDefinition CurrencyDefinition => m_CurrencyDefinition;
        public void Init()
        {
            m_Icon.sprite = m_CurrencyDefinition.Icon;
        }
        public void SetView(Currency currency)
        {
            m_CurrencyDefinition = currency.CurrencyDefinition;

            m_Icon.sprite = currency.CurrencyDefinition.Icon;
            m_AmountText.text = currency.Amount.ToString();
        }
        public void SetAmount(int amount)
        {
            m_AmountText.text = amount.ToString();
        }
    }
    public partial class GameplayPanel
    {
        private CurrencyView GetCurrencyView()
        {
            return GetBinding<CurrencyView>();
        }
        public void SetCurrencyReward(Currency currency)
        {
            GetCurrencyView().SetView(currency);
        }
    }
}
