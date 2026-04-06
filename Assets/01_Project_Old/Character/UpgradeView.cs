using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class UpgradeView : UIView
    {
        [SerializeField] private TextMeshProUGUI m_ItemNameText;
        [SerializeField] private CurrencyView m_ItemNeed;
        [SerializeField] private CurrencyView m_ItemOwned;

        private HeroUnit m_HeroUnit;

        [SerializeField] private Button m_UpgradeButton;
        [SerializeField] private Button m_QuickAccessButton;

        private Currency m_UsedUpgradeItem;
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

        public void Refresh()
        {
            InitInternal();
        }

        public void UpgradeHero()
        {
            if (!m_IsUpgradeAvailable) return;

            m_HeroUnit.AddLevel(1);

            ApplyCurrencyCost(m_UsedUpgradeItem);

            InitInternal();
        }

        private void InitInternal()
        {
            var state = EvaluateState();

            m_UsedUpgradeItem = state.RequiredCurrency;
            m_IsUpgradeAvailable = state.CanUpgrade;

            ApplyUI(state);
        }

        private UpgradeState EvaluateState()
        {
            var config = m_HeroUnit.HeroConfig.LevelFormulaConfig;

            int cost = config.GetCurrentMaxExperience(m_HeroUnit.Level);

            Currency required = new(config.ItemRequirmentConfig, cost);

            int ownedAmount = Player.Instance.CurrencyControl.GetCurrencyAmount(required.ItemConfig);
            Currency owned = new(required.ItemConfig, ownedAmount);

            bool isMaxLevel = m_HeroUnit.Level >= config.MaxLevel;
            bool enough = ownedAmount >= cost;

            return new UpgradeState
            {
                RequiredCurrency = required,
                OwnedCurrency = owned,
                CanUpgrade = enough && !isMaxLevel,
                IsMaxLevel = isMaxLevel
            };
        }

        private void ApplyUI(UpgradeState state)
        {
            m_UpgradeButton.interactable = state.CanUpgrade;
            m_QuickAccessButton.gameObject.SetActive(!state.CanUpgrade);

            m_ItemNeed.SetView(state.RequiredCurrency);
            m_ItemOwned.SetView(state.OwnedCurrency);

            m_ItemNameText.text = $"Owned {state.RequiredCurrency.ItemConfig.name}:";
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
            foreach (var statView in m_StatViews)
                statView.HideNextValue();

            base.HideInternal();
        }

        private struct UpgradeState
        {
            public Currency RequiredCurrency;
            public Currency OwnedCurrency;
            public bool CanUpgrade;
            public bool IsMaxLevel;
        }
    }
}