using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Rush;

namespace LegionKnight
{
    public partial class CurrencyView : UIView
    {
        [SerializeField]
        protected ItemConfig m_ItemConfig;
        [SerializeField]
        private Image m_Icon;
        [SerializeField]
        protected TextMeshProUGUI m_AmountText;
        public ItemConfig ItemConfig => m_ItemConfig;

        [SerializeField]
        private bool m_UseAbbreviation = true;
        [SerializeField]
        private UnityEvent<Currency> m_OnSetViewInvoke = new();
        [SerializeField]
        private UnityEvent<int> m_OnSetAmountInvoke = new();
        public void Init()
        {
            InitInternal();
        }
        protected void InitInternal()
        {
            if (m_ItemConfig == null)
            {
                Debug.LogError("Currency is null");
                return;
            }
            m_Icon.sprite = m_ItemConfig.CollectibleField.Icon;
            SetAmountInternal(Player.Instance.CurrencyControl.GetCurrencyAmount(m_ItemConfig));
        }
        protected virtual void SetViewInternal(Currency currency)
        {
            if (currency.ItemConfig == null)
            {
                Debug.LogError("Currency is null");
                return;
            }
            m_ItemConfig = currency.ItemConfig;
            m_Icon.sprite = currency.ItemConfig.CollectibleField.Icon;
            m_AmountText.text = FormatAmountText(currency.Amount);
            m_OnSetViewInvoke?.Invoke(currency);
            m_OnSetAmountInvoke?.Invoke(currency.Amount);
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
            if (m_ItemConfig == null)
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
