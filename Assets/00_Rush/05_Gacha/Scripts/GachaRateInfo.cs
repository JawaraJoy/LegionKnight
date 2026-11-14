namespace Rush
{
    public class GachaRateInfo
    {
        private readonly GachaCollectableConfig m_Collectable;
        private readonly float m_NormalizedChance;

        public GachaCollectableConfig Collectable => m_Collectable;
        public float NormalizedChance => m_NormalizedChance;
        public float Percent => m_NormalizedChance * 100f;

        internal static bool IsEligible(GachaCollectableConfig entry) =>
            entry != null && entry.Collect != null && entry.Amount > 0 && entry.Chance > 0f &&
            !float.IsInfinity(entry.Chance);

        internal static float TotalWeight(GachaCollectableConfig[] pool, GachaBannerConfig normalBanner = null)
        {
            float total = 0f;
            if (pool != null)
                foreach (var entry in pool)
                    if (IsEligible(entry)) total += normalBanner == null ? entry.Chance : normalBanner.NormalWeight(entry);
            return total;
        }

        internal static CollectibleConfig SingleReward(GachaCollectableConfig[] pool)
        {
            CollectibleConfig reward = null;
            if (pool == null) return null;
            foreach (var entry in pool)
            {
                if (!IsEligible(entry)) continue;
                if (reward != null && reward != entry.Collect) return null;
                reward = entry.Collect;
            }
            return reward;
        }

        internal static string GuaranteeTitle(GachaCollectableConfig[] pool, string fallback)
        {
            var single = SingleReward(pool);
            if (single != null) return $"{single.BaseInfo.Name} guaranteed";
            RarityConfig rarity = null;
            if (pool == null) return fallback;
            foreach (var entry in pool)
            {
                if (!IsEligible(entry)) continue;
                var next = entry.Collect.CollectibleField?.RarityConfig;
                if (next == null || (rarity != null && rarity != next)) return fallback;
                rarity = next;
            }
            return rarity == null ? fallback : $"{rarity.BaseInfo.Name} reward";
        }

        public GachaRateInfo(GachaCollectableConfig collectable, float normalizedChance)
        {
            m_Collectable = collectable;
            m_NormalizedChance = normalizedChance;
        }
    }
}

