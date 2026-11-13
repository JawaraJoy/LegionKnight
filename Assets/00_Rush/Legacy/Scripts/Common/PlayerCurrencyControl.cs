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

        internal static PlayerCurrencyControl CurrentWallet { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            if (m_CurrencyControl != null && Instance == this) CurrentWallet = m_CurrencyControl;
        }

        protected override void OnDestroy()
        {
            if (CurrentWallet == m_CurrencyControl) CurrentWallet = null;
            base.OnDestroy();
        }

        public PlayerCurrencyControl CurrencyControl => m_CurrencyControl;
    }
}
