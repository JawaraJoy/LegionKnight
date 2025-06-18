using System.Collections;
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
                int totalAmount;
                if (bonusAmount > 0 && GameManager.Instance.GetShopItemControl(shopItem).IsBonusAvaible)
                {
                    totalAmount = shopItem.Amount + bonusAmount;
                }
                else
                {
                    totalAmount = shopItem.Amount;
                    
                }
                m_Amount.text = $"{totalAmount}";
                GameManager.Instance.SetBonusAvaible(shopItem, false);
                CurrencyApplier(d, totalAmount);
                CharacterApplier(d);
                PlatformApplier(d, totalAmount);
            }
        }

        private void CurrencyApplier(Object defi, int amount)
        {
            if (defi is CurrencyDefinition currency)
            {
                m_Icon.sprite = currency.Icon;
                Player.Instance.AddCurrencyAmount(currency, amount);
            }
        }
        private void CharacterApplier(Object defi)
        {
            if (defi is CharacterDefinition character)
            {
                m_Icon.sprite = character.SmallIcon;
                bool owned = Player.Instance.GetCharacterUnit(character).Owned;
                if (owned)
                {
                    StartCoroutine(CharcterDuplicated(character));
                }
                else
                {
                    Player.Instance.SetOwned(character, true);
                }
            }
        }

        private IEnumerator CharcterDuplicated(CharacterDefinition character)
        {
            yield return new WaitForSeconds(1f);
            m_Icon.sprite = character.ShardConvert.CurrencyDefinition.Icon;
            m_Amount.text = character.ShardConvert.Amount.ToString();
            Player.Instance.AddCurrencyAmount(character.ShardConvert.CurrencyDefinition, character.ShardConvert.Amount);
        }
        private void PlatformApplier(Object defi, int amount)
        {
            if (defi is StandbyPlatformDefinition platform)
            {
                m_Icon.sprite = platform.Icon;
                Player.Instance.AddPlatformAmount(platform, amount);
            }
        }
    }
}
