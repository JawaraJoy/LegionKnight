using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class AdsAgent : MonoBehaviour
    {
        public void LoadRewardedAd()
        {
            UnityService.Instance.LoadRewardedAd();
        }
        public void LoadInterstitialAd()
        {
            UnityService.Instance.LoadInterstitialAd();
        }
        public void LoadBannerAd()
        {
            UnityService.Instance.LoadBannerAd();
        }
        public void ShowInterstitialAd()
        {
            UnityService.Instance.ShowInterstitialAd();
        }
        public void ShowRewardedAd()
        {
            UnityService.Instance.ShowRewardedAd();
        }
        public void ShowBannerAd(BannerPosition post)
        {
            UnityService.Instance.ShowBannerAd(post);
        }
        public void ShowInterstitialAd(UnityAction action)
        {
            UnityService.Instance.ShowInterstitialAd(action);
        }
        public void ShowRewardedAd(UnityAction action)
        {
            UnityService.Instance.ShowRewardedAd(action);
        }
    }
}
