using UnityEngine;

namespace LegionKnight
{
    public partial class BuyItemView : ItemView
    {
        protected override void InitInternal(object defi)
        {
            base.InitInternal(defi);
            if (defi is ShopItemDefinition shopItem)
            {
                Object d = shopItem.ItemBonus;
                int bonusAmount = shopItem.BonusAmount;
                if (bonusAmount > 0 && GameManager.Instance.GetShopItemControl(shopItem).IsBonusAvaible)
                {
                    m_Amount.text = $"{shopItem.Amount} +{bonusAmount}";
                }
                else
                {
                    m_Amount.text = $"{shopItem.Amount}";
                }
                GameManager.Instance.SetBonusAvaible(shopItem, false);
                CurrencyApplier(d);
                CharacterApplier(d);
                PlatformApplier(d);
            }
        }

        private void CurrencyApplier(Object defi)
        {
            if (defi is CurrencyDefinition currency)
            {
                m_Icon.sprite = currency.Icon;
            }
        }
        private void CharacterApplier(Object defi)
        {
            if (defi is CharacterDefinition character)
            {
                m_Icon.sprite = character.SmallIcon;
            }
        }
        private void PlatformApplier(Object defi)
        {
            if (defi is StandbyPlatformDefinition platform)
            {
                m_Icon.sprite = platform.Icon;
            }
        }
    }
}
