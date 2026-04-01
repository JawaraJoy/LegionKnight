using Rush;
using System.Collections;
using UnityEngine;

namespace LegionKnight
{
    public partial class BuyItemView : ItemView
    {
        protected override void InitInternal(CollectibleConfig collectibleConfig)
        {
            base.InitInternal(collectibleConfig);
            if (collectibleConfig is ShopItemDefinition shopItem)
            {
                CollectibleConfig d = shopItem.ItemBonus;
                int bonusAmount = shopItem.BonusAmount;
                int totalAmount;
                if (bonusAmount > 0 && GameManager.Instance.ShopManager.GetShopContainer(shopItem.ContainerName).GetShopItemControl(shopItem).IsBonusAvaible)
                {
                    totalAmount = shopItem.Amount + bonusAmount;
                }
                else
                {
                    totalAmount = shopItem.Amount;
                    
                }
                m_Amount.text = $"{totalAmount}";

                GameManager.Instance.ShopManager.GetShopContainer(shopItem.ContainerName).GetShopItemControl(shopItem).SetAvailable(false);
                CurrencyApplier(d, totalAmount);
                CharacterApplier(d);
                CardApplier(d, totalAmount);
                EnergyApplier(d, totalAmount);
            }
        }

        private void CurrencyApplier(CollectibleConfig defi, int amount)
        {
            if (defi is ItemConfig currency)
            {
                m_Icon.sprite = currency.CollectibleField.Icon;
                Player.Instance.CurrencyControl.AddCurrencyAmount(currency, amount);
            }
        }
        private void CharacterApplier(CollectibleConfig defi)
        {
            if (defi is HeroUnitConfig character)
            {
                m_Icon.sprite = character.CollectibleField.Icon;
                bool owned = Player.Instance.HeroesCollection.GetHeroUnit(character).Owned;
                if (owned)
                {
                    StartCoroutine(CharcterDuplicated(character));
                }
                else
                {
                    Player.Instance.HeroesCollection.SetOwned(character, true);
                }
            }
        }
        private IEnumerator CharcterDuplicated(HeroUnitConfig heroConfig)
        {
            yield return new WaitForSeconds(1.5f);
            m_Icon.sprite = heroConfig.ItemDuplicateConverter.ItemConfig.CollectibleField.Icon;
            m_Amount.text = heroConfig.ItemDuplicateConverter.Amount.ToString();
            ItemConfig itemConfig = heroConfig.ItemDuplicateConverter.ItemConfig;
            int amount = heroConfig.ItemDuplicateConverter.Amount;
            Player.Instance.CurrencyControl.AddCurrencyAmount(itemConfig, amount);
        }
        private void CardApplier(CollectibleConfig config, int amount)
        {
            if (config is CardConfig cardConfig)
            {
                m_Icon.sprite = cardConfig.CollectibleField.Icon;
                Player.Instance.PlayerCardDeck.AddCardAmount(cardConfig, amount);
            }
        }
        private void EnergyApplier(CollectibleConfig config, int amount)
        {
            if (config is EnergyConfig energy)
            {
                m_Icon.sprite = energy.CollectibleField.Icon;
                Player.Instance.AddEnergy(energy, amount);
            }
        }
    }
}
