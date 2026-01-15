using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public class LootField
    {
        [SerializeField]
        private ScriptableObject m_Item;
        [SerializeField]
        private bool m_IsUnique;
        [SerializeField]
        private int m_Amount;
        [SerializeField, Range(0f, 1f)]
        private float m_Chance;

        public ScriptableObject Item => m_Item;
        public bool IsUnique => m_IsUnique;
        public int Amount => m_Amount;
        public float Chance => m_Chance;

        public LootField(ScriptableObject item, bool isUnique, int amount, float chance)
        {
            m_Item = item;
            m_IsUnique = isUnique;
            m_Amount = amount;
            m_Chance = chance;
        }

        public void SetAmount(int amount)
        {
            m_Amount = amount;
        }
        public void AddAmount(int amount)
        {
            m_Amount += amount;
        }

        public void DirectTakeLoot()
        {
            CurrencyApplierInternal(m_Item, m_Amount);
            StandbyPlatformApplierInternal(m_Item, m_Amount);
            EnergyApplierInternal(m_Item, m_Amount);
            CharacterApplierInternal(m_Item);
            RandomApplierInternal(m_Item);
        }
        private static void RandomApplierInternal(ScriptableObject defi)
        {
            if (defi is LootDefinition loot)
            {
                var loots = loot.GetRandomLoots();
                foreach(LootField field in loots)
                {
                    field.DirectTakeLoot();
                }
            }
        }
        private static void CurrencyApplierInternal(ScriptableObject defi, int amount)
        {
            if (defi is CurrencyDefinition currency)
            {
                Player.Instance.AddCurrencyAmount(currency, amount);
            }
        }
        private static void CharacterApplierInternal(ScriptableObject defi)
        {
            if (defi is CharacterDefinition character)
            {
                bool owned = Player.Instance.GetCharacterUnit(character).Owned;
                if (owned)
                {
                    Player.Instance.AddCurrencyAmount(character.ShardConvert.CurrencyDefinition, character.ShardConvert.Amount);
                }
                else
                {
                    Player.Instance.SetOwned(character, true);
                }
            }
        }
        private static void StandbyPlatformApplierInternal(ScriptableObject defi, int amount)
        {
            if (defi is PlatformConfig platform)
            {
                Player.Instance.AddPlatformAmount(platform, amount);
            }
        }
        private static void EnergyApplierInternal(ScriptableObject defi, int amount)
        {
            if (defi is EnergyDefinition energy)
            {
                Player.Instance.AddEnergy(energy, amount);
            }
        }
        public static void CurrencyApplier(ScriptableObject defi, int amount)
        {
            CurrencyApplierInternal(defi, amount);
        }
        public static void CharacterApplier(ScriptableObject defi)
        {
            CharacterApplierInternal(defi);
        }
        public static void StandbyPlatformApplier(ScriptableObject defi, int amount)
        {
            StandbyPlatformApplierInternal(defi, amount);
        }
        public static void EnergyApplier(ScriptableObject defi, int amount)
        {
            EnergyApplierInternal(defi, amount);
        }
        public static void RandomApplier(ScriptableObject defi)
        {
            RandomApplierInternal(defi);
        }
    }
}
