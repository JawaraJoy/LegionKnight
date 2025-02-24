using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerCurrencyControl : CurrenciesControl
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerCurrencyControl m_CurrencyControl;

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
        }
        public void RemoveCurrencyAmount(CurrencyDefinition definition, int amount)
        {
            m_CurrencyControl.RemoveCurrencyAmount(definition, amount);
        }
    }
}
