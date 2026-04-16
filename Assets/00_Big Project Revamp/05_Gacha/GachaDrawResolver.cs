using UnityEngine;

namespace Rush
{
    public class GachaDrawResolver : MonoBehaviour
    {
        // Entry point utama — dipanggil sekali per draw
        public GachaCollectableConfig Resolve(GachaBannerConfig banner, GachaPityTracker pity)
        {
            // Final pity window → override dengan array guarantee
            if (pity.IsInFinalPityWindow && HasGuarantees(banner.FinalPityGuarantees))
            {
                var result = ResolveFromGuaranteeArrayInternal(
                    banner.FinalPityGuarantees, pity.FinalPityGuaranteeIndex);

                if (pity.ShouldResetFinalPity)
                    pity.ResetFinalPity();

                return result;
            }

            // Small pity window → override dengan array guarantee
            if (pity.IsInSmallPityWindow && HasGuarantees(banner.SmallPityGuarantees))
            {
                var result = ResolveFromGuaranteeArrayInternal(
                    banner.SmallPityGuarantees, pity.SmallPityGuaranteeIndex);

                if (pity.ShouldResetSmallPity)
                    pity.ResetSmallPity();

                return result;
            }

            // First draw guarantee
            if (pity.ShouldTriggerFirstDraw && HasGuarantees(banner.FirstDrawGuarantees))
            {
                pity.MarkFirstDrawDone();
                return ResolveRandomFromArrayInternal(banner.FirstDrawGuarantees);
            }

            return ResolveRandomInternal(banner);
        }

        // Ambil item dari slot index tertentu dalam array guarantee
        // Slot dipilih secara random dari semua item yang ada di slot tersebut
        // (karena setiap slot adalah satu GachaCollectableConfig, jadi langsung return)
        private GachaCollectableConfig ResolveFromGuaranteeArrayInternal(
            GachaCollectableConfig[] guarantees, int index)
        {
            // index sudah di-clamp oleh tracker, aman langsung akses
            return guarantees[index];
        }

        // Untuk first draw: random dari seluruh array guarantee
        private GachaCollectableConfig ResolveRandomFromArrayInternal(
            GachaCollectableConfig[] guarantees)
        {
            if (guarantees == null || guarantees.Length == 0) return null;

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

        // Normal random dari pool collectables banner
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