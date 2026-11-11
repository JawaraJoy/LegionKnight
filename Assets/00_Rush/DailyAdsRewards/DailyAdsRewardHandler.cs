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
        [SerializeField] private string m_Key = "_default";
        [SerializeField] private int m_MaxBundlesToSelect = 3;
        [SerializeField] private DailyAdsBundleConfig[] m_Bundles;
        [SerializeField, MMReadOnly] private List<DailyAdsBundleConfig> m_TodaySelectedBundle = new();
        [SerializeField] private UnityEvent<DailyAdsBundleConfig[]> m_OnBundlesUpdate = new();

        private string LastRandomize => $"{m_Key}_randomize";
        public UnityEvent<DailyAdsBundleConfig[]> OnBundlesUpdate => m_OnBundlesUpdate;
        public DailyAdsBundleConfig[] SelectedBundles => m_TodaySelectedBundle.ToArray();

        public void Init()
        {
            InitInternal();
        }

        private void InitInternal()
        {
            if (NeedRandomize())
                RandomizeBundles();
            else
                LoadSelectedBundles();
        }

        private bool NeedRandomize()
        {
            return !UnityService.Instance.HasData(LastRandomize) ||
                UnityService.Instance.GetData<string>(LastRandomize) != DateTime.Now.Date.ToString();
        }

        private void LoadSelectedBundles()
        {
            m_TodaySelectedBundle.Clear();
            var used = new HashSet<string>();
            var count = Mathf.Min(Mathf.Max(0, m_MaxBundlesToSelect), AvailableBundleCount);
            for (int i = 0; i < count; i++)
            {
                string key = $"{m_Key}_bundle_id_{i}";
                string id = UnityService.Instance.GetData<string>(key);
                DailyAdsBundleConfig selected = null;
                foreach (var candidate in m_Bundles)
                {
                    if (candidate != null && candidate.BaseInfo.Id == id && !used.Contains(id))
                    {
                        selected = candidate;
                        break;
                    }
                }

                // Old saves stored Unity object references, which cannot survive a restart.
                // Repair only the missing selection; keep today's per-slot claim flag intact.
                if (selected == null)
                {
                    foreach (var candidate in m_Bundles)
                    {
                        if (candidate != null && !used.Contains(candidate.BaseInfo.Id))
                        {
                            selected = candidate;
                            break;
                        }
                    }
                }

                if (selected == null) continue;
                used.Add(selected.BaseInfo.Id);
                m_TodaySelectedBundle.Add(selected);
                UnityService.Instance.SaveData(key, selected.BaseInfo.Id);
            }
            OnBundlesUpdateInvoke();
        }

        private int AvailableBundleCount
        {
            get
            {
                var ids = new HashSet<string>();
                if (m_Bundles != null)
                    foreach (var bundle in m_Bundles)
                        if (bundle != null) ids.Add(bundle.BaseInfo.Id);
                return ids.Count;
            }
        }

        private void RandomizeBundles()
        {
            m_TodaySelectedBundle.Clear();
            var candidates = new List<DailyAdsBundleConfig>();
            var ids = new HashSet<string>();
            if (m_Bundles != null)
                foreach (var bundle in m_Bundles)
                    if (bundle != null && ids.Add(bundle.BaseInfo.Id)) candidates.Add(bundle);

            for (int i = 0; i < m_MaxBundlesToSelect && candidates.Count > 0; i++)
            {
                int index = UnityEngine.Random.Range(0, candidates.Count);
                var bundle = candidates[index];
                m_TodaySelectedBundle.Add(bundle);
                UnityService.Instance.SaveData($"{m_Key}_bundle_id_{i}", bundle.BaseInfo.Id);
                ResetBundleClaimState(i);
                candidates.RemoveAt(index);
            }
            UnityService.Instance.SaveData(LastRandomize, DateTime.Now.Date.ToString());
            OnBundlesUpdateInvoke();
        }

        private void OnBundlesUpdateInvoke()
        {
            m_OnBundlesUpdate?.Invoke(m_TodaySelectedBundle.ToArray());
        }

        private void ResetBundleClaimState(int index)
        {
            UnityService.Instance.SaveData(BundleClaimKey(index), false);
        }

        public bool IsBundleClaimed(int index) => UnityService.Instance.GetData<bool>(BundleClaimKey(index));

        public void SetBundleClaimed(int index)
        {
            UnityService.Instance.SaveData(BundleClaimKey(index), true);
        }

        internal void GrantRewardSnapshot(DateTime requestDate, int index, string source, CollectibleResultData result)
        {
            // Refresh the current calendar state even when the panel is hidden.
            InitInternal();
            string receiptKey = $"{m_Key}_ad_receipt_{requestDate:yyyyMMdd}_{index}";
            var service = UnityService.Instance;
            if (service.GetData<bool>(receiptKey)) return;
            bool usesTodayQuota = requestDate.Date == DateTime.Now.Date;
            if (usesTodayQuota && IsBundleClaimed(index)) return;
            service.LocalSave.ExecuteTransaction(() =>
            {
                result.GrantRewards(source);
                service.SaveData(receiptKey, true);
                if (usesTodayQuota) service.SaveData(BundleClaimKey(index), true);
            });
        }

        private string BundleClaimKey(int index) => $"{m_Key}_claimed_{index}";

        public TimeSpan GetRemainingResetTime() => DateTime.Now.Date.AddDays(1) - DateTime.Now;
    }
}
