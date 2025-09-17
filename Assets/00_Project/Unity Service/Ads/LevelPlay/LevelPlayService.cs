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
    [SerializeField]
    protected UnityEvent m_OnRewardedAdDone;
}
