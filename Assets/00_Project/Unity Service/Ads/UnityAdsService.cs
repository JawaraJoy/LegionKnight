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
            m_OnInited?.Invoke();
            m_Inited = true;
        }
        public void LoadInterstitialAd()
        {
            m_OnLoaded?.Invoke();
        }
        public void LoadRewardedAd()
        {
            m_OnLoaded?.Invoke();
        }
        public void LoadBannerAd()
        {
            m_OnLoaded?.Invoke();
        }
        public void ShowInterstitialAd()
        {
            m_OnShow.Invoke(null);
        }
        public void ShowRewardedAd()
        {
            m_OnShow.Invoke(null);
        }
        public void ShowBannerAd(BannerPosition position)
        {
            m_OnBannerPositioning.Invoke(position);
        }
        public void ShowInterstitialAd(UnityAction onCompleted)
        {
            m_OnShow.Invoke(onCompleted);
        }
        public void ShowRewardedAd(UnityAction onCompleted)
        {
            m_OnShow?.Invoke(onCompleted);
        }
    }
}
