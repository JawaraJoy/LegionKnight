using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class BreakThroughView : UIView
    {
        [SerializeField] private TextMeshProUGUI m_ShardNameText;
        [SerializeField] private CurrencyView m_ShardNeed;
        [SerializeField] private CurrencyView m_CoinNeed;
        [SerializeField] private CurrencyView m_ShardOwned;
        [SerializeField] private CurrencyView m_CoinOwned;

        private HeroUnit m_HeroUnit;

        [SerializeField] private Button m_UpgradeButton;
        [SerializeField] private Button m_QuickAccessButton;

        private Currency m_UsedUpgradeShard;
        private Currency m_UsedUpgradeCoin;

        private bool m_IsUpgradeAvailable = false;

        [SerializeField] private StatView[] m_StatViews;

        private void OnEnable()
        {
            m_UpgradeButton.onClick.AddListener(UpgradeHero);
        }

        private void OnDisable()
        {
            m_UpgradeButton.onClick.RemoveListener(UpgradeHero);
        }

        public void Init(HeroUnit unit)
        {
            m_HeroUnit = unit;
            InitInternal();
        }

        private void UpgradeHero()
        {
            if (!m_IsUpgradeAvailable) return;

            m_HeroUnit.AddStar(1);

            ApplyCurrencyCost(m_UsedUpgradeShard);
            ApplyCurrencyCost(m_UsedUpgradeCoin);

            InitInternal();
            HideInternal();
        }

        private void InitInternal()
        {
            var state = EvaluateState();

            m_UsedUpgradeShard = state.ShardRequired;
            m_UsedUpgradeCoin = state.CoinRequired;
            m_IsUpgradeAvailable = state.CanBreak;

            ApplyUI(state);
        }

        private BreakState EvaluateState()
        {
            var config = m_HeroUnit.HeroConfig.BreakThroughFormulaConfig;

            int shardCost = config.GetShardCostToBreak(m_HeroUnit.Star);
            int coinCost = config.GetCoinCostToBreak(m_HeroUnit.Star);

            Currency shardReq = new(config.ShardConfig, shardCost);
            Currency coinReq = new(config.CoinConfig, coinCost);

            int ownedShard = Player.Instance.CurrencyControl.GetCurrencyAmount(config.ShardConfig);
            int ownedCoin = Player.Instance.CurrencyControl.GetCurrencyAmount(config.CoinConfig);

            bool enough = ownedShard >= shardCost && ownedCoin >= coinCost;
            bool levelOk = config.CanBreakByLevel(m_HeroUnit.Star, m_HeroUnit.Level);
            bool isMax = m_HeroUnit.Star >= m_HeroUnit.MaxStar;

            return new BreakState
            {
                ShardRequired = shardReq,
                CoinRequired = coinReq,
                ShardOwned = new Currency(config.ShardConfig, ownedShard),
                CoinOwned = new Currency(config.CoinConfig, ownedCoin),
                CanBreak = enough && levelOk && !isMax,
                IsEnoughCurrency = enough
            };
        }

        private void ApplyUI(BreakState state)
        {
            m_UpgradeButton.interactable = state.CanBreak;
            m_QuickAccessButton.gameObject.SetActive(!state.IsEnoughCurrency);

            m_ShardNeed.SetView(state.ShardRequired);
            m_CoinNeed.SetView(state.CoinRequired);

            m_ShardOwned.SetView(state.ShardOwned);
            m_CoinOwned.SetView(state.CoinOwned);

            m_ShardNameText.text = "Owned:";
        }

        private void ApplyCurrencyCost(Currency cost)
        {
            int owned = Player.Instance.CurrencyControl.GetCurrencyAmount(cost.ItemConfig);
            Player.Instance.CurrencyControl.SetCurrencyAmount(cost.ItemConfig, owned - cost.Amount);
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            foreach (var statView in m_StatViews)
                statView.ShowNextValue();
        }

        protected override void HideInternal()
        {
            base.HideInternal();
        }

        private struct BreakState
        {
            public Currency ShardRequired;
            public Currency CoinRequired;
            public Currency ShardOwned;
            public Currency CoinOwned;

            public bool CanBreak;
            public bool IsEnoughCurrency;
        }
    }
}