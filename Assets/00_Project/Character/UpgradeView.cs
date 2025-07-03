using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class UpgradeView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_ShardNameText;
        [SerializeField]
        private CurrencyView m_ShardNeed;
        [SerializeField]
        private CurrencyView m_ShardOwned;

        private CharacterUnit m_CharacterUnit;

        [SerializeField]
        private Button m_UpgradeButton;
        [SerializeField]
        private Button m_QuickAccessButton;

        private Currency m_UsedUpgradeShard;

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

        public void UpgradeHero()
        {
            if (!m_IsUpgradeAvailable) return;
            m_CharacterUnit.AddLevel(1);

            int ownShardAmount = Player.Instance.GetCurrencyAmount(m_UsedUpgradeShard.CurrencyDefinition);
            int ressShardOwned = ownShardAmount - m_UsedUpgradeShard.Amount;
            Player.Instance.SetCurrencyAmount(m_UsedUpgradeShard.CurrencyDefinition, ressShardOwned);

            InitInternal();
        }

        private void InitInternal()
        {
            CurrencyDefinition levelUpCurDefi = m_CharacterUnit.ShardDefinition;
            int levelUpCurAmount = m_CharacterUnit.CurrentMaxExp;

            Currency levelUpCurrency = new(levelUpCurDefi, levelUpCurAmount);

            bool isMaxLevel = m_CharacterUnit.Level >= m_CharacterUnit.MaxLevel;
            bool canLevelUp = Player.Instance.GetCurrencyAmount(levelUpCurDefi) >= levelUpCurAmount && !isMaxLevel;
            m_UsedUpgradeShard = levelUpCurrency;
            m_IsUpgradeAvailable = canLevelUp;
            m_UpgradeButton.interactable = canLevelUp;
            m_QuickAccessButton.gameObject.SetActive(!canLevelUp);

            int ownerCurrencyAmount = Player.Instance.GetCurrencyAmount(m_UsedUpgradeShard.CurrencyDefinition);
            Currency ownedCurrency = new(m_UsedUpgradeShard.CurrencyDefinition, ownerCurrencyAmount);

            m_ShardNeed.SetView(m_UsedUpgradeShard);

            m_ShardOwned.SetView(ownedCurrency);

            m_IsUpgradeAvailable = canLevelUp;
            m_UpgradeButton.interactable = canLevelUp;

            m_ShardNameText.text = $"Owned {m_UsedUpgradeShard.CurrencyDefinition.name}:";
            m_ShardNeed.SetView(new Currency(m_UsedUpgradeShard.CurrencyDefinition, m_UsedUpgradeShard.Amount));
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
