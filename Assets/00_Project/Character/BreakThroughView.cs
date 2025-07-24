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

        private CharacterUnit m_CharacterUnit;

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
        public void Init(CharacterUnit unit)
        {
            m_CharacterUnit = unit;


            InitInternal();
        }

        private void UpgradeHero()
        {
            if (!m_IsUpgradeAvailable) return;
            m_CharacterUnit.AddStar(1);

            int ownShardAmount = Player.Instance.GetCurrencyAmount(m_UsedUpgradeShard.CurrencyDefinition);
            int ownCoinAmount = Player.Instance.GetCurrencyAmount(m_UsedUpgradeCoin.CurrencyDefinition);
            int ressShardOwned = ownShardAmount - m_UsedUpgradeShard.Amount;
            int ressCoinOwned = ownCoinAmount - m_UsedUpgradeCoin.Amount;

            Player.Instance.SetCurrencyAmount(m_UsedUpgradeShard.CurrencyDefinition, ressShardOwned);
            Player.Instance.SetCurrencyAmount(m_UsedUpgradeCoin.CurrencyDefinition, ressCoinOwned);

            InitInternal();
            HideInternal();
            foreach (var statView in m_StatViews)
            {
                statView.HideNextValue();
            }
        }

        private void InitInternal()
        {

            CurrencyDefinition breakShardDefi = m_CharacterUnit.GetBreakCost().CurrencyDefinition;
            int breakShardAmount = m_CharacterUnit.GetBreakCost().Amount;
            CurrencyDefinition breakCoinDefi = m_CharacterUnit.GetBreakCoinCost().CurrencyDefinition;
            int breakCoinAmount = m_CharacterUnit.GetBreakCoinCost().Amount;

            Currency breakShardCurrency = new(breakShardDefi, breakShardAmount);
            Currency breakCoinCurrency = new(breakCoinDefi, breakCoinAmount);

            bool isTimeToBreak = m_CharacterUnit.CanBreak();
            bool isMaxStar = m_CharacterUnit.Star >= m_CharacterUnit.MaxStar;
            bool enoughShard = Player.Instance.GetCurrencyAmount(breakShardDefi) >= breakShardAmount;
            bool enoughCoin = Player.Instance.GetCurrencyAmount(breakCoinDefi) >= breakCoinAmount;
            bool isEnoughCurrency = enoughShard && enoughCoin;
            bool canBreak = isEnoughCurrency && isTimeToBreak && !isMaxStar;


            m_UsedUpgradeShard = breakShardCurrency;
            m_UsedUpgradeCoin = breakCoinCurrency;

            m_UpgradeButton.interactable = canBreak;
            m_QuickAccessButton.gameObject.SetActive(!isEnoughCurrency);

            int ownerShardAmount = Player.Instance.GetCurrencyAmount(m_UsedUpgradeShard.CurrencyDefinition);
            int ownerCoinAmount = Player.Instance.GetCurrencyAmount(m_UsedUpgradeCoin.CurrencyDefinition);
            Currency ownedCurrency = new(m_UsedUpgradeShard.CurrencyDefinition, ownerShardAmount);
            Currency ownedCoin = new(m_UsedUpgradeCoin.CurrencyDefinition, ownerCoinAmount);

            m_ShardNeed.SetView(m_UsedUpgradeShard);
            m_CoinNeed.SetView(m_UsedUpgradeCoin);

            m_ShardOwned.SetView(ownedCurrency);
            m_CoinOwned.SetView(ownedCoin);

            m_IsUpgradeAvailable = canBreak;
            m_UpgradeButton.interactable = m_IsUpgradeAvailable;

            m_ShardNameText.text = $"Owned:";
            m_ShardNeed.SetView(new Currency(m_UsedUpgradeShard.CurrencyDefinition, m_UsedUpgradeShard.Amount));
            m_CoinNeed.SetView(new Currency(m_UsedUpgradeCoin.CurrencyDefinition, m_UsedUpgradeCoin.Amount));
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
