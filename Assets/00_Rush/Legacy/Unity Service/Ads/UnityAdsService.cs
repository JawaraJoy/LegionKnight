using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace LegionKnight
{
    public class UnityAdsService : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private bool m_Inited = false;
        private LevelPlayService ActiveService => (this as UnityAdsManager)?.LevelPlayService;
        [SerializeField]
        private UnityEvent m_OnInited;
        [SerializeField]
        private UnityEvent m_OnLoaded;
        [SerializeField]
        private UnityEvent<UnityAction> m_OnShow = new();
        [SerializeField]
        private UnityEvent<BannerPosition> m_OnBannerPositioning;

        public void Init()
        {
            if (ActiveService != null) { ActiveService.Init(); return; }
            m_OnInited?.Invoke();
            m_Inited = true;
        }
        public void LoadInterstitialAd()
        {
            if (ActiveService != null) { ActiveService.LoadInterstitialAds(); return; }
            m_OnLoaded?.Invoke();
        }
        public void LoadRewardedAd()
        {
            if (ActiveService != null) { ActiveService.LoadRewardedAds(); return; }
            m_OnLoaded?.Invoke();
        }
        public void LoadBannerAd()
        {
            if (ActiveService != null) { ActiveService.LoadBannerAds(); return; }
            m_OnLoaded?.Invoke();
        }
        public void ShowInterstitialAd()
        {
            if (ActiveService != null) { ActiveService.ShowInternitialAds(null); return; }
            m_OnShow.Invoke(null);
        }
        public void ShowRewardedAd()
        {
            if (ActiveService != null) { ActiveService.ShowRewardedAds(null); return; }
            m_OnShow.Invoke(null);
        }
        public void ShowBannerAd(BannerPosition position)
        {
            if (ActiveService != null) { ActiveService.ShowBannerAds(position); return; }
            m_OnBannerPositioning.Invoke(position);
        }
        public void ShowInterstitialAd(UnityAction onCompleted)
        {
            if (ActiveService != null) { ActiveService.ShowInternitialAds(onCompleted); return; }
            m_OnShow.Invoke(onCompleted);
        }
        public void ShowRewardedAd(UnityAction onCompleted)
        {
            if (ActiveService != null) { ActiveService.ShowRewardedAds(onCompleted); return; }
            m_OnShow?.Invoke(onCompleted);
        }
    }
}
