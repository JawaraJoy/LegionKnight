using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class UnityAdsManager : UnityAds
    {
        
    }
    public partial class UnityService
    {
        [SerializeField]
        private UnityAdsManager m_UnityAdsManager;

        public void LoadRewardedAd()
        {
            m_UnityAdsManager.LoadRewardedAd();
        }
        public void LoadInterstitialAd()
        {
            m_UnityAdsManager.LoadInterstitialAd();
        }
        public void LoadBannerAd()
        {
            m_UnityAdsManager.LoadBannerAd();
        }
        public void ShowInterstitialAd()
        {
            m_UnityAdsManager.ShowInterstitialAd();
        }
        public void ShowRewardedAd()
        {
            m_UnityAdsManager.ShowRewardedAd();
        }
        public void ShowBannerAd(BannerPosition post)
        {
            m_UnityAdsManager.ShowBannerAd(post);
        }
        public void ShowInterstitialAd(UnityAction action)
        {
            m_UnityAdsManager.ShowInterstitialAd(action);
        }
        public void ShowRewardedAd(UnityAction action)
        {
            m_UnityAdsManager.ShowRewardedAd(action);
        }
    }
}
