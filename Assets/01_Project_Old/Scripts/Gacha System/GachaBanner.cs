using Rush;
using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class GachaBanner
    {
        [SerializeField] private BannerDefinition m_Definition;
        [SerializeField] private DrawDiscount m_SingleDiscount;
        [SerializeField] private DrawDiscount m_MultiDiscount;

        private int m_TotalDraws;
        private int m_SmallPity;
        private bool m_FirstDrawUsed;

        private LocalSave Cloud => UnityService.Instance.LocalSave;

        private string TotalKey => $"{m_Definition.Id}_total";
        private string SmallKey => $"{m_Definition.Id}_small";
        private string FirstKey => $"{m_Definition.Id}_first";

        public BannerDefinition Definition => m_Definition;

        public float GetDrawCountRate()
        {
            return (float)m_TotalDraws / m_Definition.GuaranteedDraw;
        }
        private void ConsumeDiscountIfNeeded(int drawCount)
        {
            DrawDiscount discount = drawCount > 1
                ? m_MultiDiscount
                : m_SingleDiscount;

            if (discount == null)
                return;

            if (!discount.DiscountEnabled)
                return;

            if (!discount.FirstDrawConsumed)
            {
                discount.ConsumeFirstDraw();
            }
        }
        public int TotalDraws => m_TotalDraws;
        public int GetBaseCostForCurrency(ItemConfig currency, int drawCount)
        {
            GachaCurrencyCost baseCost;

            if (Definition.MainCurrency.ItemConfig == currency)
                baseCost = Definition.MainCurrency;
            else if (Definition.AlternativeCurrency.ItemConfig == currency)
                baseCost = Definition.AlternativeCurrency;
            else
            {
                Debug.LogError($"Currency {currency.name} not supported by banner {Definition.Label}");
                return int.MaxValue;
            }

            return baseCost.Amount * drawCount;
        }
        private int GetFinalCostForCurrency(ItemConfig currency, int drawCount)
        {
            // ❌ MAIN currency TIDAK kena discount
            if (Definition.MainCurrency.ItemConfig == currency)
            {
                return Definition.MainCurrency.Amount * drawCount;
            }

            // ✅ HANYA alternative currency
            if (Definition.AlternativeCurrency.ItemConfig != currency)
            {
                Debug.LogError($"Currency {currency.name} not supported by banner {Definition.Label}");
                return int.MaxValue;
            }

            int cost = Definition.AlternativeCurrency.Amount * drawCount;

            DrawDiscount discount = drawCount > 1
                ? m_MultiDiscount
                : m_SingleDiscount;

            if (discount == null || !discount.DiscountEnabled)
                return cost;

            float rate = discount.PriceRate;

            // ✅ FIRST DRAW untuk SINGLE ATAU MULTI
            if (!discount.FirstDrawConsumed)
            {
                rate *= discount.FirstDrawRate;
            }

            return Mathf.CeilToInt(cost * rate);
        }
        public GachaCurrencyCost GetFinalCurrencyCost(ItemConfig currency, int drawCount)
        {
            int finalCost = GetFinalCostForCurrency(currency, drawCount);
            return new GachaCurrencyCost(currency, finalCost);
        }

        public void Init()
        {
            if (Cloud.HasData(TotalKey))
                m_TotalDraws = Cloud.GetDataValue<int>(TotalKey);

            if (Cloud.HasData(SmallKey))
                m_SmallPity = Cloud.GetDataValue<int>(SmallKey);

            if (Cloud.HasData(FirstKey))
                m_FirstDrawUsed = Cloud.GetDataValue<bool>(FirstKey);

            LoadDiscount(m_SingleDiscount);
            LoadDiscount(m_MultiDiscount);
        }

        public void Draw(int count, List<GachaRewardConfig> results)
        {
            for (int i = 0; i < count; i++)
            {
                var reward = RollOnce(i == 0);
                results.Add(reward);
            }

            ConsumeDiscountIfNeeded(count);

            SaveState();
        }

        private GachaRewardConfig RollOnce(bool firstIndex)
        {
            m_TotalDraws++;
            m_SmallPity++;

            if (!m_FirstDrawUsed && firstIndex && m_Definition.FirstDrawReward != null)
            {
                m_FirstDrawUsed = true;
                return m_Definition.FirstDrawReward;
            }

            if (m_TotalDraws >= m_Definition.GuaranteedDraw)
            {
                m_TotalDraws = 0;
                return RollWithSoftPity(m_Definition.MainRewards);
            }

            if (m_SmallPity >= m_Definition.SmallPity)
            {
                m_SmallPity = 0;
                return RollFrom(m_Definition.SmallPityRewards);
            }

            return RollFrom(m_Definition.NormalRewards);
        }

        private GachaRewardConfig RollFrom(IReadOnlyList<GachaRewardConfig> pool)
        {
            float total = 0f;
            foreach (var r in pool)
                total += r.Weight;

            float roll = Random.value * total;
            float acc = 0f;

            foreach (var r in pool)
            {
                acc += r.Weight;
                if (roll <= acc)
                    return r;
            }

            return pool[^1];
        }

        private GachaRewardConfig RollWithSoftPity(IReadOnlyList<GachaRewardConfig> pool)
        {
            float multiplier = 1f;

            if (m_Definition.EnableSoftPity && m_TotalDraws >= m_Definition.SoftPityStart)
            {
                int excess = m_TotalDraws - m_Definition.SoftPityStart + 1;
                multiplier += excess * m_Definition.SoftPityMultiplier * 0.01f;
            }

            float total = 0f;
            foreach (var r in pool)
                total += r.Weight * multiplier;

            float roll = Random.value * total;
            float acc = 0f;

            foreach (var r in pool)
            {
                acc += r.Weight * multiplier;
                if (roll <= acc)
                    return r;
            }

            return pool[^1];
        }

        private void SaveState()
        {
            long ttl = m_Definition.IsSeasonal
                ? m_Definition.SeasonDurationSeconds
                : 0;

            Cloud.SaveData(TotalKey, m_TotalDraws, ttl);
            Cloud.SaveData(SmallKey, m_SmallPity, ttl);
            Cloud.SaveData(FirstKey, m_FirstDrawUsed, ttl);

            SaveDiscount(m_SingleDiscount);
            SaveDiscount(m_MultiDiscount);
        }

        private void SaveDiscount(DrawDiscount discount)
        {
            if (discount == null || !discount.DiscountEnabled)
                return;

            long ttl = m_Definition.IsSeasonal
                ? m_Definition.SeasonDurationSeconds
                : 0;

            Cloud.SaveData($"{m_Definition.Id}_discount_{discount.Id}",
                discount.FirstDrawConsumed,
                ttl);
        }

        private void LoadDiscount(DrawDiscount discount)
        {
            if (discount == null)
                return;

            string key = $"{m_Definition.Id}_discount_{discount.Id}";
            if (Cloud.HasData(key))
                discount.ConsumeFirstDraw();
        }
    }
}
