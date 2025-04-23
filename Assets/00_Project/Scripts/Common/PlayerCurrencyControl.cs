using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerCurrencyControl : CurrenciesControl
    {
        public void InitPlayerCurrency()
        {
            foreach (Currency currency in m_Currencies)
            {
                if (UnityService.Instance.HasData(currency.Id))
                {
                    UnityService.Instance.LoadData(currency.Id, () => OnCurrencyLoaded(currency));
                }
                else
                {
                    UnityService.Instance.SaveData(currency.Id, currency.Amount);
                }
            }
        }

        private void OnCurrencyLoaded(Currency current)
        {
            //object data = UnityService.Instance.GetData(current.Id);
            int amount = UnityService.Instance.GetData<int>(current.Id);
            current.SetAmount(amount);
        }
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerCurrencyControl m_CurrencyControl;

        public void InitPlayerCurrency()
        {
            m_CurrencyControl.InitPlayerCurrency();
        }
        public int GetCurrencyAmount(CurrencyDefinition definition)
        {
            return m_CurrencyControl.GetCurrencyAmount(definition);
        }
        public void SetCurrencyAmount(CurrencyDefinition definition, int amount)
        {
            m_CurrencyControl.SetCurrencyAmount(definition, amount);
        }
        public void AddCurrencyAmount(CurrencyDefinition definition, int amount)
        {
            m_CurrencyControl.AddCurrencyAmount(definition, amount);
            int currentAmount = m_CurrencyControl.GetCurrencyAmount(definition);
            UnityService.Instance.SaveData(definition.Id, currentAmount);
        }
        public void RemoveCurrencyAmount(CurrencyDefinition definition, int amount)
        {
            m_CurrencyControl.RemoveCurrencyAmount(definition, amount);
        }
    }
    public partial class PlayerAgent
    {
        public void InitPlayerCurrency()
        {
            Player.Instance.InitPlayerCurrency();
        }
    }
}
