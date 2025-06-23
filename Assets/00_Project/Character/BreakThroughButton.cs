using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class BreakThroughButton : MonoBehaviour
    {
        [SerializeField]
        private CurrencyView m_ShardAmountNeed;

        [SerializeField]
        private Button m_UpgradeButton;

        private CharacterUnit m_CharacterUnit;
        [SerializeField, ReadOnly]
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

            CurrencyDefinition breakShardDefi = unit.GetBreakCost().CurrencyDefinition;
            int breakShardAmount = unit.GetBreakCost().Amount;

            Currency breakShardCurrency = new(breakShardDefi, breakShardAmount);

            bool isTimeToBreak = unit.CanBreak();
            bool isMaxStar = unit.Star >= unit.MaxStar;
            bool canBreak = Player.Instance.GetCurrencyAmount(breakShardDefi) >= breakShardAmount && isTimeToBreak && !isMaxStar;

            m_CurrencyUsed = breakShardCurrency;

            m_UpgradeButton.interactable = canBreak;

            m_ShardAmountNeed.SetView(m_CurrencyUsed);
        }

        private void ShowUpgradeView()
        {
            m_UpgradeView.Show();
            m_UpgradeView.Init(m_CharacterUnit);
        }
    }
}
