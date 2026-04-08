using LegionKnight;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class LootField
    {
        [SerializeField]
        private CollectibleConfig m_CollectibleConfig;
        [SerializeField]
        private int m_Amount;
        [SerializeField, Range(0f, 1f)]
        private float m_Chance;

        public CollectibleConfig ItemLoot => m_CollectibleConfig;
        public int Amount => m_Amount;
        public float Chance => m_Chance;

        public LootField(CollectibleConfig config, int amount, float chance)
        {
            m_CollectibleConfig = config;
            m_Amount = Mathf.Max(0, amount);
            m_Chance = chance;
        }

        public LootField Clone()
        {
            return new LootField(m_CollectibleConfig, m_Amount, m_Chance);
        }

        public void DirectTakeLoot()
        {
            CurrencyApplierInternal(m_CollectibleConfig, m_Amount);
            CardApplierInternal(m_CollectibleConfig, m_Amount);
            EnergyApplierInternal(m_CollectibleConfig, m_Amount);
            CharacterApplierInternal(m_CollectibleConfig);
            RandomApplierInternal(m_CollectibleConfig);
        }

        public void AddAmount(int amount)
        {
            m_Amount = Mathf.Max(0, m_Amount + amount);
        }

        public void SetAmount(int amount)
        {
            m_Amount = Mathf.Max(0, amount);
        }

        private static void RandomApplierInternal(CollectibleConfig collectibleConfig)
        {
            if (collectibleConfig is LootChestDefinition loot)
            {
                var loots = loot.GetRandomLoots();
                foreach (LootField config in loots)
                {
                    config.DirectTakeLoot();
                }
            }
        }

        private static void CurrencyApplierInternal(CollectibleConfig collectibleConfig, int amount)
        {
            if (collectibleConfig is ItemConfig itemConfig)
            {
                Player.Instance.CurrencyControl.AddCurrencyAmount(itemConfig, amount);
            }
        }

        private static void CharacterApplierInternal(CollectibleConfig collectibleConfig)
        {
            if (collectibleConfig is HeroUnitConfig heroConfig)
            {
                bool owned = Player.Instance.HeroesCollection.GetHeroUnit(heroConfig).Owned;
                if (owned)
                {
                    ItemConfig itemConverter = heroConfig.ItemDuplicateConverter.ItemConfig;
                    int amountConverter = heroConfig.ItemDuplicateConverter.Amount;
                    Player.Instance.CurrencyControl.AddCurrencyAmount(itemConverter, amountConverter);
                }
                else
                {
                    Player.Instance.HeroesCollection.SetOwned(heroConfig, true);
                }
            }
        }

        private static void CardApplierInternal(CollectibleConfig collectibleConfig, int amount)
        {
            if (collectibleConfig is CardConfig cardConfig)
            {
                Player.Instance.PlayerCardDeck.AddCardAmount(cardConfig, amount);
            }
        }

        private static void EnergyApplierInternal(CollectibleConfig collectibleConfig, int amount)
        {
            if (collectibleConfig is EnergyConfig energy)
            {
                Player.Instance.AddEnergy(energy, amount);
            }
        }

        public static void CurrencyApplier(CollectibleConfig collectibleConfig, int amount)
        {
            CurrencyApplierInternal(collectibleConfig, amount);
        }

        public static void CharacterApplier(CollectibleConfig collectibleConfig)
        {
            CharacterApplierInternal(collectibleConfig);
        }

        public static void StandbyPlatformApplier(CollectibleConfig collectibleConfig, int amount)
        {
            CardApplierInternal(collectibleConfig, amount);
        }

        public static void EnergyApplier(CollectibleConfig collectibleConfig, int amount)
        {
            EnergyApplierInternal(collectibleConfig, amount);
        }

        public static void RandomApplier(CollectibleConfig collectibleConfig)
        {
            RandomApplierInternal(collectibleConfig);
        }
    }
}