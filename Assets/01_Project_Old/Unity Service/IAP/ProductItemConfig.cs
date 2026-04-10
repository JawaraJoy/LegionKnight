using LegionKnight;
using UnityEngine;

namespace Rush
{
    public partial class ProductItemConfig : CollectibleConfig
    {
        [SerializeField]
        CollectibleConfig m_CollectibleConfig;
        [SerializeField]
        private int m_Amount;

        public CollectibleConfig CollectibleConfig => m_CollectibleConfig;
        public int Amount => m_Amount;

        public void AddProductToPlayer()
        {
            LootField.CharacterApplier(m_CollectibleConfig);
            LootField.CurrencyApplier(m_CollectibleConfig, m_Amount);
            LootField.EnergyApplier(m_CollectibleConfig, m_Amount);
            LootField.CardApplier(m_CollectibleConfig, m_Amount);
            LootField.RandomApplier(m_CollectibleConfig);

            /*if (m_CollectibleConfig is ItemConfig itemConfig)
            {
                Player.Instance.CurrencyControl.AddCurrencyAmount(itemConfig, m_Amount);
            }
            else if (m_CollectibleConfig is HeroUnitConfig heroConfig)
            {
                Player.Instance.HeroesCollection.SetOwned(heroConfig, true);
            }
            else if (m_CollectibleConfig is CardConfig cardConfig)
            {
                Player.Instance.PlayerCardDeck.AddCardAmount(cardConfig, m_Amount);
            }
            else if (m_CollectibleConfig is EnergyConfig ene)
            {
                Player.Instance.EnergyController.Add(ene, m_Amount);
            }
            else if (m_CollectibleConfig is CustomImageDefinition img)
            {
                Player.Instance.CustomProfile.SetOwned(img, true);
            }
            else if (m_CollectibleConfig is BadgeConfig badge)
            {
                Player.Instance.BadgeManager.SetCurrentUpgradeLevel(badge, m_Amount);
            }*/
        }
    }
}
