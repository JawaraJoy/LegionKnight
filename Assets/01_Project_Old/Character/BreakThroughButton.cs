using NaughtyAttributes;
using Rush;
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

        private HeroUnit m_HeroUnit;
        [SerializeField, ReadOnly]
        private Currency m_CurrencyUsed;
        [SerializeField, ReadOnly]
        private Currency m_CurrencyUsedCoin;
        public HeroUnit HeroUnit => m_HeroUnit;
        public Currency CurrencyUsed => m_CurrencyUsed;

        [SerializeField]
        private BreakThroughView m_UpgradeView;

        private HeroUnitConfig m_HeroConfig;

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
            if (m_HeroConfig == null)
            {
                m_HeroConfig = Player.Instance.HeroDeck.SelectedHero;
            }
            Init(m_HeroConfig);
        }
        public void Init(HeroUnitConfig heroConfig)
        {
            m_HeroConfig = heroConfig;
            HeroUnit unit = Player.Instance.HeroDeck.GetHeroUnit(heroConfig);
            m_HeroUnit = unit;

            ItemConfig breakItemConfig = heroConfig.BreakThroughFormulaConfig.ShardConfig;
            int breakItemAmount = heroConfig.BreakThroughFormulaConfig.GetShardAmountToBreak(unit.Star);

            ItemConfig breakSecondItemConfig = heroConfig.BreakThroughFormulaConfig.CoinConfig;
            int breakSecondItemAmount = heroConfig.BreakThroughFormulaConfig.GetCoinAmountToBreak(unit.Star);

            Currency breakShardCurrency = new(breakItemConfig, breakItemAmount);
            Currency breakCoinCurrency = new(breakSecondItemConfig, breakSecondItemAmount);

            bool isTimeToBreak = heroConfig.BreakThroughFormulaConfig.CanBreak(unit.Star, unit.Star);
            bool isMaxStar = unit.Star >= unit.MaxStar;
            bool enoughItem = Player.Instance.CurrencyControl.GetCurrencyAmount(breakItemConfig) >= breakItemAmount;
            bool enoughSecondItem = Player.Instance.CurrencyControl.GetCurrencyAmount(breakSecondItemConfig) >= breakSecondItemAmount;
            bool isEnoughCurrency = enoughItem && enoughSecondItem;

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
            m_UpgradeView.Init(m_HeroUnit);
        }
    }
}
