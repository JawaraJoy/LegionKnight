using NaughtyAttributes;
using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public class UpgradeButton : UIView
    {
        [SerializeField] private CurrencyView m_ShardAmountNeed;

        [SerializeField] private Button m_UpgradeButton;
        [SerializeField] private Button m_QuickAccessButton;
        [SerializeField]
        private DailyAdRewardButton m_DailyAdRewardButton; // opsional — bisa aktif saat upgrade tidak tersedia

        private HeroUnit m_CharacterUnit;

        [SerializeField, ReadOnly] private Currency m_CurrencyUsed;

        public HeroUnit CharacterUnit => m_CharacterUnit;
        public Currency CurrencyUsed => m_CurrencyUsed;

        [SerializeField] private UpgradeView m_UpgradeView;
        [SerializeField] private TextMeshProUGUI m_UpgradeButtonText;

        [SerializeField] private UnityEvent<HeroUnitConfig> m_OnInit = new();

        private HeroUnitConfig m_HeroUsed;

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
            if (m_HeroUsed == null)
                m_HeroUsed = Player.Instance.HeroesCollection.SelectedHero;

            InitInternal(m_HeroUsed);
        }

        public void Init(HeroUnitConfig heroConfig)
        {
            InitInternal(heroConfig);
        }

        private void InitInternal(HeroUnitConfig heroConfig)
        {
            m_HeroUsed = heroConfig;
            m_CharacterUnit = Player.Instance.HeroesCollection.GetHeroUnit(heroConfig);

            var state = EvaluateLevelUpState(m_CharacterUnit);

            m_CurrencyUsed = state.Currency;

            ApplyUI(state);

            m_OnInit?.Invoke(heroConfig);

            bool isHeroUnlocked = Player.Instance.HeroesCollection.GetHeroUnit(heroConfig).Owned;
            m_UpgradeButton.interactable = isHeroUnlocked;
                if (m_DailyAdRewardButton != null)
                    m_DailyAdRewardButton.gameObject.SetActive(!state.CanUpgrade && isHeroUnlocked);
        }

        private void ApplyUI(LevelUpState state)
        {
            m_ShardAmountNeed.SetView(m_CurrencyUsed);

            m_UpgradeButton.interactable = state.CanUpgrade;
            m_QuickAccessButton.gameObject.SetActive(!state.CanUpgrade);

            ApplyText(state);

            if (state.IsMaxLevel)
                m_ShardAmountNeed.Hide();
            else
                m_ShardAmountNeed.Show();
        }

        private LevelUpState EvaluateLevelUpState(HeroUnit unit)
        {
            var config = unit.HeroConfig.LevelFormulaConfig;

            int cost = config.GetCurrentMaxExperience(unit.Level);

            Currency currency = new(config.ItemRequirmentConfig, cost);

            bool isMaxLevel = unit.Level >= config.MaxLevel;

            bool enough = Player.Instance.CurrencyControl.GetCurrencyAmount(config.ItemRequirmentConfig) >= cost;

            bool canUpgrade = enough && !isMaxLevel;

            return new LevelUpState
            {
                Currency = currency,
                CanUpgrade = canUpgrade,
                IsMaxLevel = isMaxLevel
            };
        }

        private void ApplyText(LevelUpState state)
        {
            if (state.IsMaxLevel)
                m_UpgradeButtonText.text = "Max Level";
            else if (state.CanUpgrade)
                m_UpgradeButtonText.text = "Upgrade";
            else
                m_UpgradeButtonText.text = "Not Enough";
        }

        private void ShowUpgradeView()
        {
            if (m_UpgradeView == null) return;

            m_UpgradeView.Show();
            m_UpgradeView.Init(m_CharacterUnit);
        }

        private struct LevelUpState
        {
            public Currency Currency;
            public bool CanUpgrade;
            public bool IsMaxLevel;
        }
    }
}