using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance { get; private set; }

    #if UNITY_ANDROID
    string appKey = "19e606f05";
    #elif UNITY_IPHONE
    string appKey = "";
    #else
    string appKey = "unexpected_platform";
    #endif

    public UnityEvent RewardedAdsClosed;
    public bool isInstance;

    private void Awake()
    {
        if(isInstance == true){
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
    }

    private void Start()
    {
        //LoadInterstitial();
        LoadBanner();
        IronSource.Agent.validateIntegration();
        IronSource.Agent.init(appKey);

        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            //adsStatusText.text = "Ads Ready";
        }
    }

    private void OnEnable()
    {
        IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitialized;

        // Add AdInfo Banner Events
        IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;

        // Add AdInfo Interstitial Events
        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;

        // Add AdInfo Rewarded Video Events
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
    }

    void SdkInitialized()
    {
        print("Sdk is initialized!!");
    }

    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    #region banner
    public void LoadBanner()
    {
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }
    public void DestroyBanner()
    {
        IronSource.Agent.destroyBanner();
    }

    // Banner AdInfo Delegates
    void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo) { }
    void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError) { }
    void BannerOnAdClickedEvent(IronSourceAdInfo adInfo) { }
    void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo) { }
    void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo) { }
    void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo) { }
    #endregion

    #region interstitial
    public void LoadInterstitial()
    {
        IronSource.Agent.loadInterstitial();
    }
    public void ShowInterstitial()
    {
        if (IronSource.Agent.isInterstitialReady())
        {
            IronSource.Agent.showInterstitial();
        }
        else
        {
            Debug.Log("interstitial not ready!!");
        }
    }

    // Interstitial AdInfo Delegates
    void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo) { 
        ShowInterstitial();
    }
    void InterstitialOnAdLoadFailed(IronSourceError ironSourceError) { }
    void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo) { }
    void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo) { }
    void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo) { }
    void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo) { }
    void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo) { }
    #endregion

    #region rewarded
    public void LoadRewarded()
    {
        IronSource.Agent.loadRewardedVideo();
    }

    public void ShowRewarded()
    {
        //LoadingAdsPanel.SetActive(true);
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("rewarded not ready!!");
            //adsStatusText.text = "Ads not ready";
        }
    }

    // RewardedVideo AdInfo Delegates
    void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo) 
    {
        //adsStatusText.text = "Ads Ready";
    }
    void RewardedVideoOnAdUnavailable() 
    {
        //adsStatusText.text = "Ads not ready";
    }
    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo) { 
        
    }
    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo) { 
        //LoadingAdsPanel.SetActive(false);
    }
    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        
        /*int crrCoin = PlayerPrefs.GetInt("totalCoins");
        crrCoin += 100;
        PlayerPrefs.SetInt("totalCoins", crrCoin);

        totalCoinsTxt.text = PlayerPrefs.GetInt("totalCoins").ToString();
        */
        if (RewardedAdsClosed != null)
        {
            RewardedAdsClosed.Invoke();
        }

        //LoadRewarded();
        
    }
    void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo) { }
    void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo) { }
    #endregion
}
