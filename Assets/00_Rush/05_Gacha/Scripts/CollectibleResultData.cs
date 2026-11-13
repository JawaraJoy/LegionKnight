using System;
using System.Collections.Generic;

namespace Rush
{
    public class CollectibleResultData
    {
        private readonly List<CollectibleResultEntry> m_Entries = new();
        private bool m_WasSpecialDrop;
        private bool m_RewardsGranted;
        public IReadOnlyList<CollectibleResultEntry> Entries => m_Entries;
        public bool WasSpecialDrop => m_WasSpecialDrop;
        public string Source { get; private set; }
        public bool RewardsGranted => m_RewardsGranted;

        internal void AddEntry(CollectibleConfig collectible, int amount)
        {
            if (m_RewardsGranted) throw new InvalidOperationException("Committed rewards cannot be changed.");
            if (collectible == null || amount <= 0) throw new ArgumentException("Reward must have a collectible and positive amount.");
            m_Entries.Add(new CollectibleResultEntry(collectible, amount));
        }

        internal void SetSpecialDrop(bool value) => m_WasSpecialDrop |= value;

        internal void MarkPurchaseDelivered(string source)
        {
            Source = source;
            m_RewardsGranted = true;
        }

        internal void GrantRewards(string source)
        {
            if (m_RewardsGranted) return;
            foreach (var entry in m_Entries) entry.GrantReward(source);
            Source = source;
            m_RewardsGranted = true;
        }
    }

    public class CollectibleResultEntry
    {
        private readonly CollectibleConfig m_Collectible;
        private readonly int m_Amount;
        private bool m_WasDuplicateHero;
        private bool m_Granted;
        public CollectibleConfig Collectible => m_Collectible;
        public int Amount => m_Amount;
        public bool WasDuplicateHero => m_WasDuplicateHero;

        public CollectibleResultEntry(CollectibleConfig collectible, int amount)
        {
            m_Collectible = collectible;
            m_Amount = amount;
        }

        internal void GrantReward(string source)
        {
            if (m_Granted) return;
            m_WasDuplicateHero = GachaDuplicateHelper.IsDuplicateHero(m_Collectible, out _);
            CollectibleControl.AddCollectibleStatic(source, m_Collectible, m_Amount);
            m_Granted = true;
        }
    }
}

