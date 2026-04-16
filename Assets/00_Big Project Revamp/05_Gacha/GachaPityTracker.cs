using LegionKnight;
using UnityEngine;

namespace Rush
{
    // Logika pity window:
    // FinalPityCount = 50, FinalPityGuarantees.Length = 5
    // → draw ke 46,47,48,49,50 sudah masuk "pity window"
    // → tiap draw dalam window itu direplace dengan random dari FinalPityGuarantees
    // Hal yang sama berlaku untuk SmallPity
    public class GachaPityTracker : MonoBehaviour
    {
        private const string KeyPrefix = "GachaPity_";
        private const string KeySmall = "_small";
        private const string KeyFinal = "_final";
        private const string KeyFirstDone = "_firstDone";

        [SerializeField] private GachaBannerConfig m_Banner;

        private int m_SmallPityCounter;
        private int m_FinalPityCounter;
        private bool m_FirstDrawDone;

        public int SmallPityCounter => m_SmallPityCounter;
        public int FinalPityCounter => m_FinalPityCounter;
        public bool IsFirstDrawDone => m_FirstDrawDone;

        private string SmallKey => KeyPrefix + m_Banner.BaseInfo.Id + KeySmall;
        private string FinalKey => KeyPrefix + m_Banner.BaseInfo.Id + KeyFinal;
        private string FirstKey => KeyPrefix + m_Banner.BaseInfo.Id + KeyFirstDone;

        // ── Window helpers ────────────────────────────────────────────────────
        // Draw ke-N masuk final pity window jika:
        // counter setelah increment >= (FinalPityCount - guarantees.Length + 1)
        // contoh: FinalPityCount=50, guarantees=5 → window dimulai di counter 46
        private int FinalPityWindowStart =>
            m_Banner.FinalPityCount - GuaranteeLengthSafe(m_Banner.FinalPityGuarantees) + 1;

        private int SmallPityWindowStart =>
            m_Banner.SmallPityCount - GuaranteeLengthSafe(m_Banner.SmallPityGuarantees) + 1;

        private static int GuaranteeLengthSafe(GachaCollectableConfig[] arr) =>
            arr is { Length: > 0 } ? arr.Length : 1;

        // Apakah counter saat ini sudah masuk window pity?
        public bool IsInFinalPityWindow =>
            m_FinalPityCounter >= FinalPityWindowStart;

        public bool IsInSmallPityWindow =>
            m_SmallPityCounter >= SmallPityWindowStart;

        // Sudah mencapai batas maksimal (draw terakhir window)
        public bool ShouldResetFinalPity =>
            m_FinalPityCounter >= m_Banner.FinalPityCount;

        public bool ShouldResetSmallPity =>
            m_SmallPityCounter >= m_Banner.SmallPityCount;

        // Index slot dalam array guarantee (0-based)
        // contoh: window start=46, counter=47 → index 1
        public int FinalPityGuaranteeIndex =>
            Mathf.Clamp(m_FinalPityCounter - FinalPityWindowStart, 0,
                GuaranteeLengthSafe(m_Banner.FinalPityGuarantees) - 1);

        public int SmallPityGuaranteeIndex =>
            Mathf.Clamp(m_SmallPityCounter - SmallPityWindowStart, 0,
                GuaranteeLengthSafe(m_Banner.SmallPityGuarantees) - 1);

        public bool ShouldTriggerFirstDraw =>
            m_Banner.HasFirstDrawGuarantee && !m_FirstDrawDone;

        // ── Lifecycle ─────────────────────────────────────────────────────────
        private void Awake() => LoadInternal();

        public void Init(GachaBannerConfig banner)
        {
            m_Banner = banner;
            LoadInternal();
        }

        private void LoadInternal()
        {
            if (m_Banner == null) return;
            m_SmallPityCounter = UnityService.Instance.HasData(SmallKey)
                ? UnityService.Instance.GetData<int>(SmallKey) : 0;
            m_FinalPityCounter = UnityService.Instance.HasData(FinalKey)
                ? UnityService.Instance.GetData<int>(FinalKey) : 0;
            m_FirstDrawDone = UnityService.Instance.HasData(FirstKey)
                && UnityService.Instance.GetData<bool>(FirstKey);
        }

        private void SaveInternal()
        {
            UnityService.Instance.SaveData(SmallKey, m_SmallPityCounter);
            UnityService.Instance.SaveData(FinalKey, m_FinalPityCounter);
            UnityService.Instance.SaveData(FirstKey, m_FirstDrawDone);
        }

        // ── Mutations ─────────────────────────────────────────────────────────
        public void IncrementDraw()
        {
            m_SmallPityCounter++;
            m_FinalPityCounter++;
            SaveInternal();
        }

        public void ResetSmallPity()
        {
            m_SmallPityCounter = 0;
            SaveInternal();
        }

        public void ResetFinalPity()
        {
            m_FinalPityCounter = 0;
            m_SmallPityCounter = 0; // final pity reset juga small
            SaveInternal();
        }

        public void MarkFirstDrawDone()
        {
            m_FirstDrawDone = true;
            SaveInternal();
        }
    }
}