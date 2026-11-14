using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class GachaDrawResolver : MonoBehaviour
    {
        private readonly List<GachaCollectableConfig> m_Items = new();
        private bool m_WasPityTriggered;

        public IReadOnlyList<GachaCollectableConfig> Items => m_Items;
        public bool WasPityTriggered => m_WasPityTriggered;

        internal void AddItem(GachaCollectableConfig item) => m_Items.Add(item);
        internal void SetPityTriggered(bool value) => m_WasPityTriggered = value;
        // Dibaca oleh GachaHandler setelah tiap Resolve() untuk set flag pity di result
        public bool LastDrawWasPity { get; private set; }

        public GachaCollectableConfig Resolve(GachaBannerConfig banner, GachaPityTracker pity)
        {
            LastDrawWasPity = false;

            if (pity.ShouldTriggerFinalPity && GachaRateInfo.TotalWeight(banner.FinalPityGuarantees) > 0f)
            {
                pity.ResetFinalPity();
                LastDrawWasPity = true;
                return RollWeighted(banner.FinalPityGuarantees);
            }

            if (pity.ShouldTriggerSmallPity && GachaRateInfo.TotalWeight(banner.SmallPityGuarantees) > 0f)
            {
                pity.ResetSmallPity();
                LastDrawWasPity = true;
                return RollWeighted(banner.SmallPityGuarantees);
            }

            if (pity.ShouldTriggerFirstDraw && GachaRateInfo.TotalWeight(banner.FirstDrawGuarantees) > 0f)
            {
                pity.MarkFirstDrawDone();
                LastDrawWasPity = true;
                return RollWeighted(banner.FirstDrawGuarantees);
            }

            return RollWeighted(banner.Collectables, banner);
        }

        private static GachaCollectableConfig RollWeighted(GachaCollectableConfig[] pool, GachaBannerConfig normalBanner = null)
        {
            float total = GachaRateInfo.TotalWeight(pool, normalBanner);
            if (total <= 0f) return null;

            float roll = Random.Range(0f, total);
            float cumulative = 0f;
            GachaCollectableConfig last = null;
            foreach (var entry in pool)
            {
                if (!GachaRateInfo.IsEligible(entry)) continue;
                last = entry;
                cumulative += normalBanner == null ? entry.Chance : normalBanner.NormalWeight(entry);
                if (roll < cumulative) return entry;
            }
            // Unity's floating-point Random.Range can also return its upper bound.
            return last;
        }
    }
}

