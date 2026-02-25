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
            m_Amount = amount;
            m_Chance = chance;
        }
        public void DirectTakeLoot()
        {
            CurrencyApplierInternal(m_CollectibleConfig, m_Amount);
            StandbyPlatformApplierInternal(m_CollectibleConfig, m_Amount);
            EnergyApplierInternal(m_CollectibleConfig, m_Amount);
            CharacterApplierInternal(m_CollectibleConfig);
            RandomApplierInternal(m_CollectibleConfig);
        }
        public void AddAmount(int amount)
        {
            m_Amount += amount;
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
                bool owned = Player.Instance.HeroDeck.GetHeroUnit(heroConfig).Owned;
                if (owned)
                {
                    ItemConfig itemConverter = heroConfig.ItemDuplicateConverter.ItemConfig;
                    int amountConverter = heroConfig.ItemDuplicateConverter.Amount;
                    Player.Instance.CurrencyControl.AddCurrencyAmount(itemConverter, amountConverter);
                }
                else
                {
                    Player.Instance.HeroDeck.SetOwned(heroConfig, true);
                }
            }
        }
        private static void StandbyPlatformApplierInternal(CollectibleConfig collectibleConfig, int amount)
        {
            if (collectibleConfig is PlatformConfig platform)
            {
                Player.Instance.PlatformDeck.AddPlatformAmount(platform, amount);
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
            StandbyPlatformApplierInternal(collectibleConfig, amount);
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
