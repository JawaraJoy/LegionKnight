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
        [SerializeField]
        private CurrencyView m_ShardAmountNeed;

        [SerializeField]
        private Button m_UpgradeButton;
        [SerializeField]
        private Button m_QuickAccessButton;

        private HeroUnit m_CharacterUnit;
        [SerializeField, ReadOnly]
        private Currency m_CurrencyUsed;
        public HeroUnit CharacterUnit => m_CharacterUnit;
        public Currency CurrencyUsed => m_CurrencyUsed;

        [SerializeField]
        private UpgradeView m_UpgradeView;

        [SerializeField]
        private TextMeshProUGUI m_UpgradeButtonText;

        [SerializeField]
        private UnityEvent<HeroUnitConfig> m_OnInit = new();

        private HeroUnitConfig m_HeroUsed;
        private void OnEnable()
        {
            //m_CharacterUsed = Player.Instance.UsedCharacter;
            m_UpgradeButton.onClick.AddListener(ShowUpgradeView);
        }
        private void OnDisable()
        {
            m_UpgradeButton.onClick.RemoveListener(ShowUpgradeView);
        }

        public void Refresh()
        {
            if (m_HeroUsed == null)
            {
                m_HeroUsed = Player.Instance.HeroDeck.SelectedHero;
            }
            InitInternal(m_HeroUsed);
        }
        private void InitInternal(HeroUnitConfig heroConfig)
        {
            m_HeroUsed = heroConfig;
            HeroUnit unit = Player.Instance.HeroDeck.GetHeroUnit(heroConfig);
            m_CharacterUnit = unit;

            ItemConfig levelUpItemRequirment = unit.LevelFormulaDefinition.ItemRequirmentConfig;
            int levelUpCurAmount = unit.LevelFormulaDefinition.GetCurrentMaxExperience(unit.Level);

            Currency levelUpCurrency = new(levelUpItemRequirment, levelUpCurAmount);

            bool isMaxLevel = unit.Level >= unit.LevelFormulaDefinition.MaxLevel;
            bool canLevelUp = Player.Instance.CurrencyControl.GetCurrencyAmount(levelUpItemRequirment) >= levelUpCurAmount && !isMaxLevel;

            m_CurrencyUsed = levelUpCurrency;


            m_ShardAmountNeed.SetView(m_CurrencyUsed);
            if (canLevelUp)
            {
                m_UpgradeButtonText.text = "Upgrade";
                m_ShardAmountNeed.Show();
                m_UpgradeButton.interactable = canLevelUp;

            }
            else
            {
                m_ShardAmountNeed.Hide();
                m_UpgradeButton.interactable = false;
            }
            if (isMaxLevel)
            {
                m_UpgradeButtonText.text = "Max Level";
            }
            m_QuickAccessButton.gameObject.SetActive(!canLevelUp);
            m_OnInit?.Invoke(heroConfig);
        }
        public void Init(HeroUnitConfig heroConfig)
        {
            InitInternal(heroConfig);
        }

        private void ShowUpgradeView()
        {
            if (m_UpgradeView == null) return;
            m_UpgradeView.Show();
            m_UpgradeView.Init(m_CharacterUnit);
        }
    }
}
