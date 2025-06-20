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

        private CharacterUnit m_CharacterUnit;
        private Currency m_CurrencyUsed;
        public CharacterUnit CharacterUnit => m_CharacterUnit;
        public Currency CurrencyUsed => m_CurrencyUsed;

        [SerializeField]
        private UpgradeView m_UpgradeView;

        private void OnEnable()
        {
            m_UpgradeButton.onClick.AddListener(ShowUpgradeView);
        }
        private void OnDisable()
        {
            m_UpgradeButton.onClick.RemoveListener(ShowUpgradeView);
        }
        public void Init(CharacterDefinition defi)
        {
            CharacterUnit unit = Player.Instance.GetCharacterUnit(defi);
            m_CharacterUnit = unit;
            CurrencyDefinition shardDefinition = unit.ShardDefinition;
            int shardAmount = unit.CurrentMaxExp;

            Currency shardCurrency = new(shardDefinition, shardAmount);
            m_ShardAmountNeed.SetView(shardCurrency);

            m_CurrencyUsed = shardCurrency;

            bool isUpgradeAvailable = Player.Instance.GetCurrencyAmount(shardDefinition) >= shardAmount;
            m_UpgradeButton.interactable = isUpgradeAvailable;

            
        }

        private void ShowUpgradeView()
        {
            m_UpgradeView.Show();
            m_UpgradeView.Init(this);
        }
    }
}
