using System.Threading.Tasks;
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
                    //UnityService.Instance.LoadData(currency.Id, () => OnCurrencyLoaded(currency));
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
            //object data = UnityService.Instance.GetData(current.Id);
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
