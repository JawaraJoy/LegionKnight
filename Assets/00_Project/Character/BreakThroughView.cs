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
        private CurrencyView m_ShardOwned;

        private CharacterUnit m_CharacterUnit;

        [SerializeField]
        private Button m_UpgradeButton;

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

        private void UpgradeHero()
        {
            if (!m_IsUpgradeAvailable) return;
            m_CharacterUnit.AddStar(1);

            int ownShardAmount = Player.Instance.GetCurrencyAmount(m_UsedUpgradeShard.CurrencyDefinition);
            int ressShardOwned = ownShardAmount - m_UsedUpgradeShard.Amount;
            Player.Instance.SetCurrencyAmount(m_UsedUpgradeShard.CurrencyDefinition, ressShardOwned);

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

            Currency breakShardCurrency = new(breakShardDefi, breakShardAmount);

            bool isTimeToBreak = m_CharacterUnit.CanBreak();
            bool isMaxStar = m_CharacterUnit.Star >= m_CharacterUnit.MaxStar;
            bool canBreak = Player.Instance.GetCurrencyAmount(breakShardDefi) >= breakShardAmount && isTimeToBreak && !isMaxStar;


            m_UsedUpgradeShard = breakShardCurrency;

            m_UpgradeButton.interactable = canBreak;

            int ownerCurrencyAmount = Player.Instance.GetCurrencyAmount(m_UsedUpgradeShard.CurrencyDefinition);
            Currency ownedCurrency = new(m_UsedUpgradeShard.CurrencyDefinition, ownerCurrencyAmount);

            m_ShardNeed.SetView(m_UsedUpgradeShard);

            m_ShardOwned.SetView(ownedCurrency);

            m_IsUpgradeAvailable = canBreak;
            m_UpgradeButton.interactable = m_IsUpgradeAvailable;

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
                //statView.HideNextValue();
            }
            base.HideInternal();
        }
    }
}
