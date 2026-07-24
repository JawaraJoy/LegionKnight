using Rush;
using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class LevelPlayService : LevelPlaySample
    {
        public void ShowRewardedAds(UnityAction onRewardAdd)
        {
            bool hasLoad = RewardedAdInternal.IsAdReady();
            if (hasLoad)
            {
                ShowRewardedAddsInternal(onRewardAdd);
            }
            else
            {
                RewardedAdInternal.LoadAd();
            }
        }
        public void ShowInternitialAds(UnityAction onRewardAdd)
        {
            bool hasLoad = InterstitialAdInternal.IsAdReady();
            if (hasLoad)
            {
                ShowInternitialAdsInternal(onRewardAdd);
            }
            else
            {
                InterstitialAdInternal.LoadAd();
            }
        }
        private void ShowInternitialAdsInternal(UnityAction onRewardAdd)
        {
            // Implementation for interstitial ads if needed
            m_OnInterstitialAdDone?.RemoveAllListeners();
            m_OnInterstitialAdDone.AddListener(onRewardAdd);
            m_OnInterstitialAdDone.AddListener(InterstitialAdInternal.LoadAd);
            InterstitialAdInternal.ShowAd();
        }

        private void ShowRewardedAddsInternal(UnityAction onRewardAdd)
        {
            m_OnRewardedAdDone?.RemoveAllListeners();
            m_OnRewardedAdDone.AddListener(onRewardAdd);
            m_OnRewardedAdDone.AddListener(RewardedAdInternal.LoadAd);
            RewardedAdInternal.ShowAd();
        }
        public void LoadRewardedAds()
        {
            LoadToShowRewardedAddsInternal();
        }
        private void LoadToShowRewardedAddsInternal()
        {
            RewardedAdInternal.LoadAd();
        }
    }
}

public partial class LevelPlaySample
{
    protected LevelPlayRewardedAd RewardedAdInternal => rewardedVideoAd;
    protected LevelPlayInterstitialAd InterstitialAdInternal => interstitialAd;
    [SerializeField]
    protected UnityEvent m_OnRewardedAdDone;
    [SerializeField]
    protected UnityEvent m_OnInterstitialAdDone;

    protected virtual void OnRewardedAdDoneInvoke(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        m_OnRewardedAdDone?.Invoke();
        double revenue = adInfo.Revenue.Value;
        AnalyticService.Instance.WatchAds(adInfo.PlacementName, revenue);
    }
}
