using NaughtyAttributes;
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

        private CharacterUnit m_CharacterUnit;
        [SerializeField, ReadOnly]
        private Currency m_CurrencyUsed;
        public CharacterUnit CharacterUnit => m_CharacterUnit;
        public Currency CurrencyUsed => m_CurrencyUsed;

        [SerializeField]
        private UpgradeView m_UpgradeView;

        [SerializeField]
        private TextMeshProUGUI m_UpgradeButtonText;

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

            CurrencyDefinition levelUpCurDefi = unit.ShardDefinition;
            int levelUpCurAmount = unit.CurrentMaxExp;

            Currency levelUpCurrency = new(levelUpCurDefi, levelUpCurAmount);

            bool isMaxLevel = unit.Level >= unit.MaxLevel;
            bool canLevelUp = Player.Instance.GetCurrencyAmount(levelUpCurDefi) >= levelUpCurAmount && !isMaxLevel;

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
                m_UpgradeButtonText.text = "Max Level";
                m_ShardAmountNeed.Hide();
                m_UpgradeButton.interactable = false;
            }
        }

        private void ShowUpgradeView()
        {
            m_UpgradeView.Show();
            m_UpgradeView.Init(m_CharacterUnit);
        }
    }
}
