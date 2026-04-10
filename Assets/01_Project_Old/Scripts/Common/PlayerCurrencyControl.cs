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

        public PlayerCurrencyControl CurrencyControl => m_CurrencyControl;
    }
}