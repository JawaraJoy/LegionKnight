using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rush
{
    public class SellPanel : PanelView
    {
        [SerializeField]
        private Image m_ItemToSellIcon;
        [SerializeField]
        private TextMeshProUGUI m_AmountToSell;
        [SerializeField]
        private Image m_SellCurrencyIcon;
        [SerializeField]
        private TextMeshProUGUI m_AmountCurrencyToSell;

        [SerializeField]
        private Button m_CancelButton;
        [SerializeField]
        private Button m_SellButton;

        private CurrenciesControl m_CurrenciesControl;
        private void Awake()
        {
            m_SellButton.onClick.AddListener(Sell);
            m_CancelButton.onClick.AddListener(HideInternal);
        }

        private void Start()
        {
            m_CurrenciesControl = Player.Instance.CurrencyControl;

            m_CurrenciesControl.OnSelectedSellChanged.AddListener(UpdateSelectedSell);
        }
        public void AddAmount(int add)
        {
            m_CurrenciesControl.AddSelectedSellAmount(add);
        }
        public void SetAmount(int amount)
        {
            m_CurrenciesControl.SetSelectedSellAmount(amount);
        }
        public void SetToMax()
        {
            m_CurrenciesControl.SetToMax();
        }
        private void UpdateSelectedSell(CollectibleConfig config, int amount, int maxAmount)
        {
            m_ItemToSellIcon.sprite = config.CollectibleField.Icon;
            m_AmountToSell.text = $"{amount}/{maxAmount}";

            Currency currencyToSell = config.GetSellValue(amount);
            m_SellCurrencyIcon.sprite = currencyToSell.ItemConfig.CollectibleField.Icon;
            m_AmountCurrencyToSell.text = currencyToSell.Amount.ToString();
        }

        public void OpenSell(CollectibleConfig config, int amount, UnityAction<int> onSold)
        {
            m_CurrenciesControl.SetSellTarget(config, amount, onSold);
            ShowInternal();
        }
        private void Sell()
        {
            m_CurrenciesControl.Sell();
            HideInternal();

            
        }
    }
}
