using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class SelectedShopItemView : UIView
    {
        [SerializeField]
        private Image m_MainIcon;
        [SerializeField]
        private TextMeshProUGUI m_ItemNameText;
        [SerializeField]
        private TextMeshProUGUI m_ItemAmountText;
        [SerializeField]
        private TextMeshProUGUI m_BonusText;
        [SerializeField]
        private TextMeshProUGUI m_SpendCrestText;
        [SerializeField]
        private TextMeshProUGUI m_BuyButtonText;

        private ShopItemDefinition m_Definition;
        [SerializeField]
        private UnityEvent<ShopItemDefinition> m_OnBought = new();
        public void SetShow(ShopItemDefinition definition)
        {
            m_Definition = definition;
            m_ItemNameText.text = definition.ItemName;
            m_MainIcon.sprite = definition.Icon;
            bool hasBonus = definition.BonusAmount > 0 && GameManager.Instance.GetShopItemControl(definition).IsBonusAvaible;
            m_BonusText.gameObject.SetActive(hasBonus);
            m_BonusText.text = $"Bonus +{definition.BonusAmount}";
            m_ItemAmountText.text = $"x{m_Definition.Amount}";
            m_SpendCrestText.text = $"{definition.BuyButtonText} To Get +{definition.SpendRewardAmount}";
            m_BuyButtonText.text = definition.BuyButtonText;

            ShowInternal();
        }

        public void OnBoughtInvoke()
        {
            m_OnBought?.Invoke(m_Definition);
            m_Definition.Buy();
        }

    }

    public partial class ShopPanel
    {
        public void ShowSelectedShopItem(ShopItemDefinition definition)
        {
            if (GetBinding<SelectedShopItemView>() == null) return;
            GetBinding<SelectedShopItemView>().SetShow(definition);
        }
    }
}
