using LegionKnight;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class DailyAdsRewardHandler : MonoBehaviour
    {
        [Header("Persistence")]
        [SerializeField] private string m_Key = "_default";

        private string LastRandomize => $"{m_Key}_randomize";

        [SerializeField]
        private int m_MaxBundlesToSelect = 3;
        [SerializeField]
        private DailyAdsBundleConfig[] m_Bundles;
        [SerializeField, MMReadOnly]
        private List<DailyAdsBundleConfig> m_TodaySelectedBundle = new List<DailyAdsBundleConfig>();
        private DailyAdsRewardPanel m_Panel;

        [SerializeField]
        private UnityEvent<DailyAdsBundleConfig[]> m_OnBundlesUpdate;
        public UnityEvent<DailyAdsBundleConfig[]> OnBundlesUpdate => m_OnBundlesUpdate;
        private DailyAdsRewardPanel Panel
        {
            get
            {
                if (m_Panel == null)
                    m_Panel = CanvasManager.Instance.GetPanel<DailyAdsRewardPanel>();
                return m_Panel;
            }
        }
        public void Init()
        {
            InitInternal();
        }

        private void InitInternal()
        {
            if (UnityService.Instance.HasData(LastRandomize))
            {
                var lastRandomize = UnityService.Instance.GetData<int>(LastRandomize);
                if (lastRandomize == System.DateTime.Now.Day)
                {
                    LoadSelectedBundles();
                    return;
                }
            }
            RandomizeBundles();
        }

        private void LoadSelectedBundles()
        {
            m_TodaySelectedBundle.Clear();
            for (int i = 0; i < m_MaxBundlesToSelect; i++)
            {
                var bundle = UnityService.Instance.GetData<DailyAdsBundleConfig>($"{m_Key}_bundle_{i}");
                if (bundle != null)
                {
                    m_TodaySelectedBundle.Add(bundle);
                }
            }
            m_OnBundlesUpdate?.Invoke(m_TodaySelectedBundle.ToArray());
        }
        private void RandomizeBundles()
        {
            m_TodaySelectedBundle.Clear();
            var bundles = new List<DailyAdsBundleConfig>(m_Bundles);
            for (int i = 0; i < m_MaxBundlesToSelect; i++)
            {
                if (bundles.Count == 0)
                    break;
                int randomIndex = Random.Range(0, bundles.Count);
                var selectedBundle = bundles[randomIndex];
                m_TodaySelectedBundle.Add(selectedBundle);
                UnityService.Instance.SaveData($"{m_Key}_bundle_{i}", selectedBundle);
                bundles.RemoveAt(randomIndex);
            }
            UnityService.Instance.SaveData(LastRandomize, System.DateTime.Now.Day);
            
        }
    }
}
