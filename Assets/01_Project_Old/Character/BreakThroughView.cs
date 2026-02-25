using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class BreakThroughView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_ShardNameText;
        [SerializeField]
        private CurrencyView m_ShardNeed;
        [SerializeField]
        private CurrencyView m_CoinNeed;
        [SerializeField]
        private CurrencyView m_ShardOwned;
        [SerializeField]
        private CurrencyView m_CoinOwned;

        private HeroUnit m_HeroUnit;

        [SerializeField]
        private Button m_UpgradeButton;
        [SerializeField]
        private Button m_QuickAccessButton;

        private Currency m_UsedUpgradeShard;
        private Currency m_UsedUpgradeCoin;

        private bool m_IsUpgradeAvailable = false;

        [SerializeField]
        private StatView[] m_StatViews;

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

            int ownShardAmount = Player.Instance.CurrencyControl.GetCurrencyAmount(m_UsedUpgradeShard.ItemConfig);
            int ownCoinAmount = Player.Instance.CurrencyControl.GetCurrencyAmount(m_UsedUpgradeCoin.ItemConfig);
            int ressShardOwned = ownShardAmount - m_UsedUpgradeShard.Amount;
            int ressCoinOwned = ownCoinAmount - m_UsedUpgradeCoin.Amount;

            Player.Instance.CurrencyControl.SetCurrencyAmount(m_UsedUpgradeShard.ItemConfig, ressShardOwned);
            Player.Instance.CurrencyControl.SetCurrencyAmount(m_UsedUpgradeCoin.ItemConfig, ressCoinOwned);

            InitInternal();
            HideInternal();
            foreach (var statView in m_StatViews)
            {
                statView.HideNextValue();
            }
        }

        private void InitInternal()
        {

            ItemConfig breakShardDefi = m_HeroUnit.HeroConfig.BreakThroughFormulaConfig.ShardConfig;
            int breakShardAmount = m_HeroUnit.HeroConfig.BreakThroughFormulaConfig.GetShardAmountToBreak(m_HeroUnit.Star);
            ItemConfig breakCoinDefi = m_HeroUnit.HeroConfig.BreakThroughFormulaConfig.CoinConfig;
            int breakCoinAmount = m_HeroUnit.HeroConfig.BreakThroughFormulaConfig.GetCoinAmountToBreak(m_HeroUnit.Star);

            Currency breakShardCurrency = new(breakShardDefi, breakShardAmount);
            Currency breakCoinCurrency = new(breakCoinDefi, breakCoinAmount);

            bool isTimeToBreak = m_HeroUnit.HeroConfig.BreakThroughFormulaConfig.CanBreak(m_HeroUnit.Star, m_HeroUnit.Level);
            bool isMaxStar = m_HeroUnit.Star >= m_HeroUnit.MaxStar;
            bool enoughShard = Player.Instance.CurrencyControl.GetCurrencyAmount(breakShardDefi) >= breakShardAmount;
            bool enoughCoin = Player.Instance.CurrencyControl.GetCurrencyAmount(breakCoinDefi) >= breakCoinAmount;
            bool isEnoughCurrency = enoughShard && enoughCoin;
            bool canBreak = isEnoughCurrency && isTimeToBreak && !isMaxStar;


            m_UsedUpgradeShard = breakShardCurrency;
            m_UsedUpgradeCoin = breakCoinCurrency;

            m_UpgradeButton.interactable = canBreak;
            m_QuickAccessButton.gameObject.SetActive(!isEnoughCurrency);

            int ownerShardAmount = Player.Instance.CurrencyControl.GetCurrencyAmount(m_UsedUpgradeShard.ItemConfig);
            int ownerCoinAmount = Player.Instance.CurrencyControl.GetCurrencyAmount(m_UsedUpgradeCoin.ItemConfig);
            Currency ownedCurrency = new(m_UsedUpgradeShard.ItemConfig, ownerShardAmount);
            Currency ownedCoin = new(m_UsedUpgradeCoin.ItemConfig, ownerCoinAmount);

            m_ShardNeed.SetView(m_UsedUpgradeShard);
            m_CoinNeed.SetView(m_UsedUpgradeCoin);

            m_ShardOwned.SetView(ownedCurrency);
            m_CoinOwned.SetView(ownedCoin);

            m_IsUpgradeAvailable = canBreak;
            m_UpgradeButton.interactable = m_IsUpgradeAvailable;

            m_ShardNameText.text = $"Owned:";
            m_ShardNeed.SetView(new Currency(m_UsedUpgradeShard.ItemConfig, m_UsedUpgradeShard.Amount));
            m_CoinNeed.SetView(new Currency(m_UsedUpgradeCoin.ItemConfig, m_UsedUpgradeCoin.Amount));
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            foreach (var statView in m_StatViews)
            {
                statView.ShowNextValue();
            }
        }
        protected override void HideInternal()
        {
            foreach (var statView in m_StatViews)
            {
                //statView.HideNextValue();
            }
            base.HideInternal();
        }
    }
}
