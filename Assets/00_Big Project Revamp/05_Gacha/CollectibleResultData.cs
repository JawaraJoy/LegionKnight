using System.Collections.Generic;

namespace Rush
{
    public class CollectibleResultData
    {
        private readonly List<CollectibleResultEntry> m_Entries = new();
        private bool m_WasSpecialDrop;

        public IReadOnlyList<CollectibleResultEntry> Entries => m_Entries;
        public bool WasSpecialDrop => m_WasSpecialDrop;

        internal void AddEntry(CollectibleConfig collectible, int amount)
        {
            m_Entries.Add(new CollectibleResultEntry(collectible, amount));
            LootField.CharacterApplier(collectible);
            LootField.CurrencyApplier(collectible, amount);
            LootField.CardApplier(collectible, amount);
            LootField.EnergyApplier(collectible, amount);
        }
            

        // untuk gacha: dipanggil jika pity triggered
        // untuk shop: tidak dipakai (default false)
        internal void SetSpecialDrop(bool value) => m_WasSpecialDrop = value;
    }

    public class CollectibleResultEntry
    {
        private readonly CollectibleConfig m_Collectible;
        private readonly int m_Amount;

        public CollectibleConfig Collectible => m_Collectible;
        public int Amount => m_Amount;

        public CollectibleResultEntry(CollectibleConfig collectible, int amount)
        {
            m_Collectible = collectible;
            m_Amount = amount;
        }
    }
}