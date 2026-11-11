using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerCurrencyControl : CurrenciesControl
    {

    }
    // for you know this is singleton
    // you can get this just RushGameManager.Instance.CurrencyControl to get it
    public partial class Player
    {
        [SerializeField]
        private PlayerCurrencyControl m_CurrencyControl;

        public PlayerCurrencyControl CurrencyControl => m_CurrencyControl;
    }
}