using UnityEngine;

namespace Rush
{
    public class GachaDrawResolver : MonoBehaviour
    {
        // Dibaca oleh GachaHandler setelah tiap Resolve() untuk set flag pity di result
        public bool LastDrawWasPity { get; private set; }

        public GachaCollectableConfig Resolve(GachaBannerConfig banner, GachaPityTracker pity)
        {
            LastDrawWasPity = false;

            if (pity.ShouldTriggerFinalPity && HasGuarantees(banner.FinalPityGuarantees))
            {
                pity.ResetFinalPity();
                LastDrawWasPity = true;
                return ResolveRandomFromArrayInternal(banner.FinalPityGuarantees);
            }

            if (pity.ShouldTriggerSmallPity && HasGuarantees(banner.SmallPityGuarantees))
            {
                pity.ResetSmallPity();
                LastDrawWasPity = true;
                return ResolveRandomFromArrayInternal(banner.SmallPityGuarantees);
            }

            if (pity.ShouldTriggerFirstDraw && HasGuarantees(banner.FirstDrawGuarantees))
            {
                pity.MarkFirstDrawDone();
                LastDrawWasPity = true;
                return ResolveRandomFromArrayInternal(banner.FirstDrawGuarantees);
            }

            return ResolveRandomInternal(banner);
        }

        private GachaCollectableConfig ResolveRandomFromArrayInternal(
            GachaCollectableConfig[] guarantees)
        {
            float total = 0f;
            foreach (var g in guarantees) total += g.Chance;

            float roll = Random.Range(0f, total);
            float cumulative = 0f;
            foreach (var g in guarantees)
            {
                cumulative += g.Chance;
                if (roll <= cumulative) return g;
            }
            return guarantees[^1];
        }

        private GachaCollectableConfig ResolveRandomInternal(GachaBannerConfig banner)
        {
            if (banner.Collectables == null || banner.Collectables.Length == 0) return null;

            float total = 0f;
            foreach (var c in banner.Collectables) total += c.Chance;

            float roll = Random.Range(0f, total);
            float cumulative = 0f;
            foreach (var c in banner.Collectables)
            {
                cumulative += c.Chance;
                if (roll <= cumulative) return c;
            }
            return banner.Collectables[^1];
        }

        private static bool HasGuarantees(GachaCollectableConfig[] arr) =>
            arr is { Length: > 0 };
    }
}