using NaughtyAttributes;
using Rush;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class BreakThroughButton : UIView
    {
        [SerializeField] private CurrencyView m_ShardAmountNeed;
        [SerializeField] private CurrencyView m_CoinAmountNeed;

        [SerializeField] private Button m_UpgradeButton;
        [SerializeField] private Button m_QuickAccessButton;

        private HeroUnit m_HeroUnit;

        [SerializeField, ReadOnly] private Currency m_CurrencyUsed;
        [SerializeField, ReadOnly] private Currency m_CurrencyUsedCoin;

        public HeroUnit HeroUnit => m_HeroUnit;
        public Currency CurrencyUsed => m_CurrencyUsed;

        [SerializeField] private BreakThroughView m_UpgradeView;

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
                m_HeroConfig = Player.Instance.HeroesCollection.SelectedHero;

            Init(m_HeroConfig);
        }

        public void Init(HeroUnitConfig heroConfig)
        {
            m_HeroConfig = heroConfig;
            m_HeroUnit = Player.Instance.HeroesCollection.GetHeroUnit(heroConfig);

            var state = EvaluateBreakState(m_HeroUnit);

            m_CurrencyUsed = state.ShardCurrency;
            m_CurrencyUsedCoin = state.CoinCurrency;

            ApplyUI(state);
        }

        private void ApplyUI(BreakState state)
        {
            m_UpgradeButton.interactable = state.CanBreak;
            m_QuickAccessButton.gameObject.SetActive(!state.IsEnoughCurrency);

            m_ShardAmountNeed.SetView(m_CurrencyUsed);
            m_CoinAmountNeed.SetView(m_CurrencyUsedCoin);
        }

        private BreakState EvaluateBreakState(HeroUnit unit)
        {
            var config = unit.HeroConfig.BreakThroughFormulaConfig;

            int shardCost = config.GetShardCostToBreak(unit.Star);
            int coinCost = config.GetCoinCostToBreak(unit.Star);

            Currency shardCurrency = new(config.ShardConfig, shardCost);
            Currency coinCurrency = new(config.CoinConfig, coinCost);

            bool isMaxStar = unit.Star >= unit.MaxStar;

            bool enoughShard = Player.Instance.CurrencyControl.GetCurrencyAmount(config.ShardConfig) >= shardCost;
            bool enoughCoin = Player.Instance.CurrencyControl.GetCurrencyAmount(config.CoinConfig) >= coinCost;

            bool isEnoughCurrency = enoughShard && enoughCoin;

            bool levelOk = config.CanBreakByLevel(unit.Star, unit.Level);

            bool canBreak = isEnoughCurrency && levelOk && !isMaxStar;

            return new BreakState
            {
                CanBreak = canBreak,
                IsEnoughCurrency = isEnoughCurrency,
                ShardCurrency = shardCurrency,
                CoinCurrency = coinCurrency
            };
        }

        private void ShowUpgradeView()
        {
            if (m_UpgradeView == null) return;

            m_UpgradeView.Show();
            m_UpgradeView.Init(m_HeroUnit);
        }

        private struct BreakState
        {
            public bool CanBreak;
            public bool IsEnoughCurrency;
            public Currency ShardCurrency;
            public Currency CoinCurrency;
        }
    }
}