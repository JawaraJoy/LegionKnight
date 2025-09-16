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
                LoadToShowRewardedAddsInternal(() => ShowRewardedAddsInternal(onRewardAdd));
            }
        }

        private void ShowRewardedAddsInternal(UnityAction onRewardAdd)
        {
            m_OnRewardedAdDone?.RemoveAllListeners();
            m_OnRewardedAdDone.AddListener(onRewardAdd);
            m_OnRewardedAdDone.AddListener(RewardedAdInternal.LoadAd);
            RewardedAdInternal.ShowAd();
        }

        private void LoadToShowRewardedAddsInternal(UnityAction onLoaded)
        {
            m_OnLoadedDone?.RemoveAllListeners();
            m_OnLoadedDone.AddListener(onLoaded);
            RewardedAdInternal.LoadAd();
        }
    }
}

public partial class LevelPlaySample
{
    protected LevelPlayRewardedAd RewardedAdInternal => rewardedVideoAd;

    [SerializeField]
    protected UnityEvent m_OnLoadedDone;
    [SerializeField]
    protected UnityEvent m_OnRewardedAdDone;
}
