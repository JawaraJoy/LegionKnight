using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class CurrencyView : UIView
    {
        [SerializeField]
        protected CurrencyDefinition m_CurrencyDefinition;
        [SerializeField]
        private Image m_Icon;
        [SerializeField]
        protected TextMeshProUGUI m_AmountText;
        public CurrencyDefinition CurrencyDefinition => m_CurrencyDefinition;

        [SerializeField]
        private bool m_UseAbbreviation = true;
        [SerializeField]
        private UnityEvent m_OnSetViewInvoke = new UnityEvent();
        public void Init()
        {
            if (m_CurrencyDefinition == null)
            {
                Debug.LogError("Currency is null");
                return;
            }
            m_Icon.sprite = m_CurrencyDefinition.Icon;
            SetAmountInternal(Player.Instance.GetCurrencyAmount(m_CurrencyDefinition));
        }
        protected virtual void SetViewInternal(Currency currency)
        {
            if (currency.CurrencyDefinition == null)
            {
                Debug.LogError("Currency is null");
                return;
            }
            m_CurrencyDefinition = currency.CurrencyDefinition;
            m_Icon.sprite = currency.CurrencyDefinition.Icon;
            m_AmountText.text = FormatAmountText(currency.Amount);
            m_OnSetViewInvoke?.Invoke();
        }
        public virtual void SetView(Currency currency)
        {
            SetViewInternal(currency);
        }
        public void SetAmount(int amount)
        {
            SetAmountInternal(amount);
        }
        protected void SetAmountInternal(int amount)
        {
            if (m_CurrencyDefinition == null)
            {
                Debug.LogError("Currency is null");
                return;
            }
            m_AmountText.text = FormatAmountText(amount);
        }
        private string FormatAmountText(int amount)
        {
            if (m_UseAbbreviation)
            {
                return FormatAmount.Abbreviation(amount);
            }
            return amount.ToString();
        }
    }

    public static class FormatAmount
    {
        public static string Abbreviation(int amount)
        {
            if (amount >= 1000)
            {
                return (amount / 1000f).ToString("0.#") + "k";
            }
            return amount.ToString();
        }

    }
}
