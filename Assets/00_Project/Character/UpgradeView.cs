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

        private CurrencyDefinition m_ShardUsedDefi;
        private int m_ShardNeedAmount;

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
        public void Init(UpgradeButton button)
        {
            m_CharacterUnit = button.CharacterUnit;
            

            InitInternal();
        }

        public void UpgradeHero()
        {
            if (!m_IsUpgradeAvailable) return;
            m_CharacterUnit.AddLevel(1);

            int ownShardAmount = Player.Instance.GetCurrencyAmount(m_ShardUsedDefi);
            int ressShardOwned = ownShardAmount - m_ShardNeedAmount;
            Player.Instance.SetCurrencyAmount(m_ShardUsedDefi, ressShardOwned);

            InitInternal();
        }

        private void InitInternal()
        {
            CurrencyDefinition shardDefinition = m_CharacterUnit.ShardDefinition;
            int shardAmount = m_CharacterUnit.CurrentMaxExp;

            m_ShardNeedAmount = shardAmount;
            m_ShardUsedDefi = shardDefinition;


            m_ShardNameText.text = $"Owned {m_ShardUsedDefi.name}:";
            m_ShardNeed.SetView(new Currency(m_ShardUsedDefi, m_ShardNeedAmount));

            int ownerCurrencyAmount = Player.Instance.GetCurrencyAmount(m_ShardUsedDefi);

            Currency ownedCurrency = new(m_ShardUsedDefi, ownerCurrencyAmount);

            m_ShardOwned.SetView(ownedCurrency);

            m_IsUpgradeAvailable = Player.Instance.GetCurrencyAmount(m_ShardUsedDefi) >= m_ShardNeedAmount;
            m_UpgradeButton.interactable = m_IsUpgradeAvailable;
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
