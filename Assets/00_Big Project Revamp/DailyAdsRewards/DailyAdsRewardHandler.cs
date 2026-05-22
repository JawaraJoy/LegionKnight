using LegionKnight;
using MoreMountains.Tools;
using System;
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
        private List<DailyAdsBundleConfig> m_TodaySelectedBundle = new();

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
            if (NeedRandomize())
            {
                RandomizeBundles();
            }
            else
            {
                LoadSelectedBundles();
            }
        }

        private bool NeedRandomize()
        {
            if (!UnityService.Instance.HasData(LastRandomize))
                return true;

            var lastRandomize = UnityService.Instance.GetData<string>(LastRandomize);

            return lastRandomize != DateTime.Now.Date.ToString();
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

                int randomIndex = UnityEngine.Random.Range(0, bundles.Count);

                var selectedBundle = bundles[randomIndex];

                m_TodaySelectedBundle.Add(selectedBundle);

                UnityService.Instance.SaveData($"{m_Key}_bundle_{i}", selectedBundle);

                ResetBundleClaimState(i);

                bundles.RemoveAt(randomIndex);
            }

            UnityService.Instance.SaveData(LastRandomize, DateTime.Now.Date.ToString());

            m_OnBundlesUpdate?.Invoke(m_TodaySelectedBundle.ToArray());
        }

        private void ResetBundleClaimState(int index)
        {
            UnityService.Instance.SaveData(GetBundleClaimKey(index), false);
        }

        public bool IsBundleClaimed(int index)
        {
            return UnityService.Instance.GetData<bool>(GetBundleClaimKey(index));
        }

        public void SetBundleClaimed(int index)
        {
            UnityService.Instance.SaveData(GetBundleClaimKey(index), true);
        }

        private string GetBundleClaimKey(int index)
        {
            return $"{m_Key}_claimed_{index}";
        }

        public TimeSpan GetRemainingResetTime()
        {
            DateTime nextReset = DateTime.Now.Date.AddDays(1);
            return nextReset - DateTime.Now;
        }
    }
}