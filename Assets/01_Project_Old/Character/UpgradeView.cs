using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class UpgradeView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_ItemNameText;
        [SerializeField]
        private CurrencyView m_ItemNeed;
        [SerializeField]
        private CurrencyView m_ItemOwned;

        private HeroUnit m_HeroUnit;

        [SerializeField]
        private Button m_UpgradeButton;
        [SerializeField]
        private Button m_QuickAccessButton;

        private Currency m_UsedUpgradeItem;

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
        public void Refresh()
        {
            InitInternal();
        }

        public void UpgradeHero()
        {
            if (!m_IsUpgradeAvailable) return;
            m_HeroUnit.AddLevel(1);

            int ownShardAmount = Player.Instance.CurrencyControl.GetCurrencyAmount(m_UsedUpgradeItem.ItemConfig);
            int ressShardOwned = ownShardAmount - m_UsedUpgradeItem.Amount;
            Player.Instance.CurrencyControl.SetCurrencyAmount(m_UsedUpgradeItem.ItemConfig, ressShardOwned);

            InitInternal();
        }

        private void InitInternal()
        {
            ItemConfig levelIUpConfig = m_HeroUnit.HeroConfig.LevelFormulaConfig.ItemRequirmentConfig;
            int levelUpCurAmount = m_HeroUnit.HeroConfig.LevelFormulaConfig.GetCurrentMaxExperience(m_HeroUnit.Level);

            Currency levelUpCurrency = new(levelIUpConfig, levelUpCurAmount);

            bool isMaxLevel = m_HeroUnit.Level >= m_HeroUnit.HeroConfig.Progression.MaxLevel;
            bool canLevelUp = Player.Instance.CurrencyControl.GetCurrencyAmount(levelIUpConfig) >= levelUpCurAmount && !isMaxLevel;
            m_UsedUpgradeItem = levelUpCurrency;
            m_IsUpgradeAvailable = canLevelUp;
            m_UpgradeButton.interactable = canLevelUp;
            m_QuickAccessButton.gameObject.SetActive(!canLevelUp);

            int ownerCurrencyAmount = Player.Instance.CurrencyControl.GetCurrencyAmount(m_UsedUpgradeItem.ItemConfig);
            Currency ownedCurrency = new(m_UsedUpgradeItem.ItemConfig, ownerCurrencyAmount);

            m_ItemNeed.SetView(m_UsedUpgradeItem);

            m_ItemOwned.SetView(ownedCurrency);

            m_IsUpgradeAvailable = canLevelUp;
            m_UpgradeButton.interactable = canLevelUp;

            m_ItemNameText.text = $"Owned {m_UsedUpgradeItem.ItemConfig.name}:";
            m_ItemNeed.SetView(new Currency(m_UsedUpgradeItem.ItemConfig, m_UsedUpgradeItem.Amount));
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
                statView.HideNextValue();
            }
            base.HideInternal();
        }
    }
}
