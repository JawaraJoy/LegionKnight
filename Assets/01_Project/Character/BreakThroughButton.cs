using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class BreakThroughButton : UIView
    {
        [SerializeField]
        private CurrencyView m_ShardAmountNeed;
        [SerializeField]
        private CurrencyView m_CoinAmountNeed;

        [SerializeField]
        private Button m_UpgradeButton;
        [SerializeField]
        private Button m_QuickAccessButton;

        private CharacterUnit m_CharacterUnit;
        [SerializeField, ReadOnly]
        private Currency m_CurrencyUsed;
        [SerializeField, ReadOnly]
        private Currency m_CurrencyUsedCoin;
        public CharacterUnit CharacterUnit => m_CharacterUnit;
        public Currency CurrencyUsed => m_CurrencyUsed;

        [SerializeField]
        private BreakThroughView m_UpgradeView;

        private CharacterDefinition m_CharacterDefinition;

        private void OnEnable()
        {
            m_UpgradeButton.onClick.AddListener(ShowUpgradeView);
        }
        private void OnDisable()
        {
            m_UpgradeButton.onClick.RemoveListener(ShowUpgradeView);
        }
        public void Refresh()
        {
            if (m_CharacterDefinition == null)
            {
                m_CharacterDefinition = Player.Instance.SelectedCharacter;
            }
            Init(m_CharacterDefinition);
        }
        public void Init(CharacterDefinition defi)
        {
            m_CharacterDefinition = defi;
            CharacterUnit unit = Player.Instance.GetCharacterUnit(defi);
            m_CharacterUnit = unit;

            CurrencyDefinition breakShardDefi = unit.GetBreakCost().CurrencyDefinition;
            int breakShardAmount = unit.GetBreakCost().Amount;

            CurrencyDefinition breakCoinDefi = unit.GetBreakCoinCost().CurrencyDefinition;
            int breakCoinAmount = unit.GetBreakCoinCost().Amount;

            Currency breakShardCurrency = new(breakShardDefi, breakShardAmount);
            Currency breakCoinCurrency = new(breakCoinDefi, breakCoinAmount);

            bool isTimeToBreak = unit.CanBreak();
            bool isMaxStar = unit.Star >= unit.MaxStar;
            bool enoughShard = Player.Instance.GetCurrencyAmount(breakShardDefi) >= breakShardAmount;
            bool enoughCoin = Player.Instance.GetCurrencyAmount(breakCoinDefi) >= breakCoinAmount;
            bool isEnoughCurrency = enoughShard && enoughCoin;

            bool canBreak = isEnoughCurrency && isTimeToBreak && !isMaxStar;

            m_QuickAccessButton.gameObject.SetActive(!isEnoughCurrency);

            m_CurrencyUsed = breakShardCurrency;
            m_CurrencyUsedCoin = breakCoinCurrency;

            m_UpgradeButton.interactable = canBreak;

            m_ShardAmountNeed.SetView(m_CurrencyUsed);
            m_CoinAmountNeed.SetView(m_CurrencyUsedCoin);
        }

        private void ShowUpgradeView()
        {
            m_UpgradeView.Show();
            m_UpgradeView.Init(m_CharacterUnit);
        }
    }
}
