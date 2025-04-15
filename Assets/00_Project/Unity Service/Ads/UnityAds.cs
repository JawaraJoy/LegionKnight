using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class UnityAds : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [SerializeField]
        private UnityAdSetting m_AdSetting;

        [SerializeField]
        private UnityEvent m_OnAdShowCompleted = new();
        [SerializeField]
        private UnityEvent m_OnAdShowFailed = new();
        [SerializeField]
        private UnityEvent m_OnAdLoadFailed = new();

        private void Awake()
        {
            if (m_AdSetting == null)
            {
                Debug.LogError("Ad Setting is not assigned.");
                return;
            }
            InitializeAdsInternal();
        }
        private void OnAdShowCompletedInvoke()
        {
            m_OnAdShowCompleted?.Invoke();
        }
        private void OnAdLoadFailedInvoke()
        {
            m_OnAdLoadFailed?.Invoke();
        }
        private void OnAdShowFailedInvoke()
        {
            m_OnAdShowFailed?.Invoke();
        }
        private void OnAdShowCompletedAddListerner(UnityAction action)
        {
            m_OnAdShowCompleted.RemoveAllListeners();
            m_OnAdShowCompleted.AddListener(action);
        }
        private void OnAdShowCompletedRemoveListerner(UnityAction action)
        {
            m_OnAdShowCompleted.RemoveListener(action);
        }
        private void InitializeAdsInternal()
        {
            Advertisement.Initialize(GetAdGameId(), GetTestMode(), this);
        }
        public void LoadInterstitialAd()
        {
            Advertisement.Load(GetInterstitialID(), this);
        }
        public void LoadRewardedAd()
        {
            Advertisement.Load(GetRewardedID(), this);
        }
        public void LoadBannerAd()
        {
            Advertisement.Load(GetBannerID(), this);
        }
        public void ShowInterstitialAd()
        {
            if (!Advertisement.isInitialized)
            {
                Debug.LogError("Interstitial Ad is not ready.");
                return;
            }
            Advertisement.Show(GetInterstitialID(), this);
        }
        public void ShowRewardedAd()
        {
            if (!Advertisement.isInitialized)
            {
                Debug.LogError("Rewarded Ad is not ready.");
                return;
            }
            Advertisement.Show(GetRewardedID(), this);
        }
        public void ShowBannerAd(BannerPosition position)
        {
            if (!Advertisement.isInitialized)
            {
                Debug.LogError("Banner Ad is not ready.");
                return;
            }
            Advertisement.Banner.SetPosition(position);
            Advertisement.Banner.Show(GetBannerID());
        }
        public void ShowInterstitialAd(UnityAction onCompleted)
        {
            if (!Advertisement.isInitialized)
            {
                Debug.LogError("Interstitial Ad is not ready.");
                return;
            }
            OnAdShowCompletedAddListerner(onCompleted);
            Advertisement.Show(GetInterstitialID(), this);
        }
        public void ShowRewardedAd(UnityAction onCompleted)
        {
            if (!Advertisement.isInitialized)
            {
                Debug.LogError("Rewarded Ad is not ready.");
                return;
            }
            Advertisement.Show(GetRewardedID(), this);
            OnAdShowCompletedAddListerner(onCompleted);
        }
        private MobileDevice GetDevice()
        {
            return Application.platform == RuntimePlatform.Android ? MobileDevice.Android : MobileDevice.IOS;
        }
        private bool GetTestMode()
        {
            return m_AdSetting.GetTestMode(GetDevice());
        }
        private string GetAdGameId()
        {
            return m_AdSetting.GetAdGameId(GetDevice());
        }
        private string GetInterstitialID()
        {
            return m_AdSetting.GetInterstitialID(GetDevice());
        }
        private string GetBannerID()
        {
            return m_AdSetting.GetBannerID(GetDevice());
        }
        private string GetRewardedID()
        {
            return m_AdSetting.GetRewardedID(GetDevice());
        }
        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads Initialization Complete");
            Advertisement.Load(GetInterstitialID(), this);
            Advertisement.Load(GetBannerID(), this);
            Advertisement.Load(GetRewardedID(), this);
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.LogError($"Unity Ads Initialization Failed: {error} - {message}");
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            Debug.Log($"Unity Ads Ad Loaded: {placementId}");
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            Debug.LogError($"Unity Ads Failed to Load: {placementId} - {error} - {message}");
            OnAdLoadFailedInvoke();
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.LogError($"Unity Ads Show Failure: {placementId} - {error} - {message}");
            OnAdShowFailedInvoke();
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            Debug.Log($"Unity Ads Show Start: {placementId} - {placementId}"); // TODO: Check if this is correct
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            Debug.Log($"Unity Ads Show Click: {placementId} - {placementId}");
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {   
            Debug.Log($"Unity Ads Show Complete: {placementId} - {showCompletionState}");
            OnAdShowCompletedInvoke();
            Advertisement.Load(GetInterstitialID(), this);
            Advertisement.Load(GetBannerID(), this);
            Advertisement.Load(GetRewardedID(), this);
        }
    }
}
