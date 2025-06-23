using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public class UpgradeButton : UIView
    {
        [SerializeField]
        private CurrencyView m_ShardAmountNeed;

        [SerializeField]
        private Button m_UpgradeButton;

        private CharacterUnit m_CharacterUnit;
        [SerializeField, ReadOnly]
        private Currency m_CurrencyUsed;
        public CharacterUnit CharacterUnit => m_CharacterUnit;
        public Currency CurrencyUsed => m_CurrencyUsed;

        [SerializeField]
        private UpgradeView m_UpgradeView;

        private void OnEnable()
        {
            m_UpgradeButton.onClick.AddListener(ShowUpgradeView);
        }
        private void OnDisable()
        {
            m_UpgradeButton.onClick.RemoveListener(ShowUpgradeView);
        }
        public void Init(CharacterDefinition defi)
        {
            CharacterUnit unit = Player.Instance.GetCharacterUnit(defi);
            m_CharacterUnit = unit;
            CurrencyDefinition levelUpCurDefi = unit.ShardDefinition;
            int levelUpCurAmount = unit.CurrentMaxExp;

            Currency levelUpCurrency = new(levelUpCurDefi, levelUpCurAmount);
            

            CurrencyDefinition breakShardDefi = unit.GetBreakCost(unit.Star).CurrencyDefinition;
            int breakShardAmount = unit.GetBreakCost(unit.Star).Amount;

            Currency breakShardCurrency = new(breakShardDefi, breakShardAmount);

            

            bool isTimeToBreak = unit.CanBreak(unit.Star, unit.Level);
            bool isMaxStar = unit.Star >= unit.MaxStar;
            bool canBreak = Player.Instance.GetCurrencyAmount(breakShardDefi) >= breakShardAmount && isTimeToBreak && isMaxStar;

            bool isMaxLevel = unit.Level >= unit.MaxLevel;
            bool canLevelUp = Player.Instance.GetCurrencyAmount(levelUpCurDefi) >= levelUpCurAmount && !isMaxLevel || canBreak;


            if (canBreak)
            {
                m_CurrencyUsed = breakShardCurrency;
            }
            else
            {
                m_CurrencyUsed = levelUpCurrency;
            }

            m_UpgradeButton.interactable = canLevelUp;

            m_ShardAmountNeed.SetView(m_CurrencyUsed);
        }

        private void ShowUpgradeView()
        {
            m_UpgradeView.Show();
            m_UpgradeView.Init(this);
        }
    }
}
