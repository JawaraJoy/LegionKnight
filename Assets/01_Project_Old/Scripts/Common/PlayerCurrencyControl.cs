using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerCurrencyControl : CurrenciesControl
    {
        public void InitPlayerCurrency()
        {
            foreach (Currency currency in m_Currencies)
            {
                if (UnityService.Instance.HasData(currency.ItemConfig.BaseInfo.Id))
                {
                    OnCurrencyLoaded(currency);
                }
                else
                {
                    UnityService.Instance.SaveData(currency.ItemConfig.BaseInfo.Id, currency.Amount);
                }
            }
        }

        private void OnCurrencyLoaded(Currency currency)
        {
            int amount = UnityService.Instance.GetData<int>(currency.ItemConfig.BaseInfo.Id);
            currency.SetAmount(amount);
        }
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerCurrencyControl m_CurrencyControl;

        public PlayerCurrencyControl CurrencyControl => m_CurrencyControl;
    }
}