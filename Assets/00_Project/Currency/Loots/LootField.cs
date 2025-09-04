using UnityEngine;

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
            CurrencyApplier(m_Item, m_Amount);
            StandbyPlatformApplier(m_Item, m_Amount);
            EnergyApplier(m_Item, m_Amount);
            CharacterApplier(m_Item);
        }

        private void CurrencyApplier(ScriptableObject defi, int amount)
        {
            if (defi is CurrencyDefinition currency)
            {
                Player.Instance.AddCurrencyAmount(currency, amount);
            }
        }
        private void CharacterApplier(ScriptableObject defi)
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
        private void StandbyPlatformApplier(ScriptableObject defi, int amount)
        {
            if (defi is StandbyPlatformDefinition platform)
            {
                Player.Instance.AddPlatformAmount(platform, amount);
            }
        }
        private void EnergyApplier(ScriptableObject defi, int amount)
        {
            if (defi is EnergyDefinition energy)
            {
                Player.Instance.AddEnergy(energy, amount);
            }
        }
    }
}
