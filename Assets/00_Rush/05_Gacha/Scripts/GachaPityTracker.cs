using LegionKnight;
using UnityEngine;

namespace Rush
{
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

        // Tepat di draw ke SmallPityCount → trigger
        public bool ShouldTriggerSmallPity => m_SmallPityCounter >= m_Banner.SmallPityCount;
        // Tepat di draw ke FinalPityCount → trigger
        public bool ShouldTriggerFinalPity => m_FinalPityCounter >= m_Banner.FinalPityCount;
        public bool ShouldTriggerFirstDraw =>
            m_Banner.HasFirstDrawGuarantee && !m_FirstDrawDone;

        private string SmallKey => KeyPrefix + m_Banner.BaseInfo.Id + KeySmall;
        private string FinalKey => KeyPrefix + m_Banner.BaseInfo.Id + KeyFinal;
        private string FirstKey => KeyPrefix + m_Banner.BaseInfo.Id + KeyFirstDone;

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
            m_SmallPityCounter = 0;
            SaveInternal();
        }

        public void MarkFirstDrawDone()
        {
            m_FirstDrawDone = true;
            SaveInternal();
        }
    }
}