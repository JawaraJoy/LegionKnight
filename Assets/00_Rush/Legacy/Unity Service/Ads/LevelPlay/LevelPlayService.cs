using System;
using Rush;
using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Advertisements;

namespace LegionKnight
{
    // Keeps the existing script GUID and serialized quest events.
    public class LevelPlayService : MonoBehaviour
    {
        [SerializeField] private UnityEvent m_OnRewardedAdDone = new();
        [SerializeField] private UnityEvent m_OnInterstitialAdDone = new();
        private const float c_LoadTimeout = 30f;
        private const float c_RetryDelay = 5f;
        private const float c_CloseGrace = 1f;
        private LevelPlayRewardedAd m_Rewarded;
        private LevelPlayInterstitialAd m_Interstitial;
        private LevelPlayBannerAd m_Banner;
        private bool m_InitRequested;
        private bool m_Initialized;
        private bool m_RewardedLoading;
        private bool m_InterstitialLoading;
        private bool m_BannerRequested;
        private bool m_Pending;
        private bool m_Showing;
        private bool m_RewardGranted;
        private bool m_Closed;
        private bool m_InterstitialRequest;
        private UnityAction m_Callback;
        private float m_Deadline;
        private float m_RetryAt;
        private string m_AdId;
        private AdsWaitPanel m_WaitPanel;
        private BannerPosition m_BannerPosition = BannerPosition.BOTTOM_CENTER;
        public bool IsInitialized => m_Initialized;
        public bool HasActiveRequest => m_Pending || m_Showing;

        public void Init() { InitInternal(); }
        private void InitInternal()
        {
            if (m_InitRequested || !isActiveAndEnabled) return;
            m_InitRequested = true;
            LevelPlay.OnInitSuccess += HandleInitSuccess;
            LevelPlay.OnInitFailed += HandleInitFailure;
            try { LevelPlay.Init(AdConfig.AppKey); }
            catch (Exception exception) { FailInitializationInternal(exception.Message); }
        }
        private void HandleInitSuccess(LevelPlayConfiguration config)
        {
            if (!isActiveAndEnabled || m_Initialized) return;
            m_Initialized = true;
            m_Rewarded = new LevelPlayRewardedAd(AdConfig.RewardedVideoAdUnitId);
            m_Interstitial = new LevelPlayInterstitialAd(AdConfig.InterstitalAdUnitId);
            CreateBannerInternal();
            m_Rewarded.OnAdLoaded += HandleRewardedLoaded;
            m_Rewarded.OnAdLoadFailed += HandleRewardedLoadFailed;
            m_Rewarded.OnAdDisplayed += HandleRewardedDisplayed;
            m_Rewarded.OnAdRewarded += HandleReward;
            m_Rewarded.OnAdClosed += HandleRewardedClosed;
            m_Rewarded.OnAdDisplayFailed += HandleRewardedDisplayFailed;
            m_Interstitial.OnAdLoaded += HandleInterstitialLoaded;
            m_Interstitial.OnAdLoadFailed += HandleInterstitialLoadFailed;
            m_Interstitial.OnAdDisplayed += HandleInterstitialDisplayed;
            m_Interstitial.OnAdClosed += HandleInterstitialClosed;
            m_Interstitial.OnAdDisplayFailed += HandleInterstitialDisplayFailed;
            m_Banner.OnAdLoadFailed += HandleBannerLoadFailed;
            LoadRewardedInternal();
            LoadInterstitialInternal();
            if (m_BannerRequested) m_Banner.LoadAd();
            TryShowInternal();
        }
        private void HandleInitFailure(LevelPlayInitError error) { FailInitializationInternal(error.ToString()); }
        private void FailInitializationInternal(string message)
        {
            LevelPlay.OnInitSuccess -= HandleInitSuccess;
            LevelPlay.OnInitFailed -= HandleInitFailure;
            m_InitRequested = false;
            m_Initialized = false;
            FailRequestInternal(message);
        }
        public void ShowRewardedAds(UnityAction callback) { RequestInternal(false, callback); }
        public void ShowInternitialAds(UnityAction callback) { RequestInternal(true, callback); }
        private void RequestInternal(bool interstitial, UnityAction callback)
        {
            if (!isActiveAndEnabled) { Debug.LogWarning("Ads service is disabled."); return; }
            if (HasActiveRequest) { Debug.LogWarning("An ad request is already active."); return; }
            m_Pending = true;
            m_InterstitialRequest = interstitial;
            m_Callback = callback;
            m_RewardGranted = false;
            m_Closed = false;
            m_AdId = null;
            m_Deadline = Time.realtimeSinceStartup + c_LoadTimeout;
            SetStatusInternal("Loading ad...", true);
            InitInternal();
            TryShowInternal();
        }
        private void TryShowInternal()
        {
            if (!m_Pending || !m_Initialized) return;
            bool ready = m_InterstitialRequest ? m_Interstitial.IsAdReady() : m_Rewarded.IsAdReady();
            if (!ready)
            {
                if (m_InterstitialRequest) LoadInterstitialInternal(); else LoadRewardedInternal();
                return;
            }
            m_Pending = false;
            m_Showing = true;
            ClearStatusInternal();
            try
            {
                if (m_InterstitialRequest) m_Interstitial.ShowAd(); else m_Rewarded.ShowAd();
            }
            catch (Exception exception) { FailRequestInternal(exception.Message); }
        }
        private void Update()
        {
            if (m_Pending && Time.realtimeSinceStartup >= m_Deadline) FailRequestInternal("Ad loading timed out. Please try again.");
            if (m_Closed && Time.realtimeSinceStartup >= m_Deadline) FinishRequestInternal();
            if (m_Initialized && !HasActiveRequest && Time.realtimeSinceStartup >= m_RetryAt)
            {
                m_RetryAt = Time.realtimeSinceStartup + c_RetryDelay;
                LoadRewardedInternal();
                LoadInterstitialInternal();
            }
        }
        public void LoadRewardedAds() { InitInternal(); LoadRewardedInternal(); }
        public void LoadInterstitialAds() { InitInternal(); LoadInterstitialInternal(); }
        public void LoadBannerAds() { m_BannerRequested = true; InitInternal(); if (m_Initialized) m_Banner.LoadAd(); }
        public void ShowBannerAds(BannerPosition position)
        {
            if (m_BannerPosition != position && m_Banner != null)
            {
                m_Banner.OnAdLoadFailed -= HandleBannerLoadFailed;
                m_Banner.DestroyAd();
                m_BannerPosition = position;
                CreateBannerInternal();
                m_Banner.OnAdLoadFailed += HandleBannerLoadFailed;
            }
            m_BannerPosition = position;
            LoadBannerAds();
        }
        private void CreateBannerInternal()
        {
            var position = m_BannerPosition switch
            {
                BannerPosition.TOP_LEFT => LevelPlayBannerPosition.TopLeft,
                BannerPosition.TOP_CENTER => LevelPlayBannerPosition.TopCenter,
                BannerPosition.TOP_RIGHT => LevelPlayBannerPosition.TopRight,
                BannerPosition.BOTTOM_LEFT => LevelPlayBannerPosition.BottomLeft,
                BannerPosition.BOTTOM_RIGHT => LevelPlayBannerPosition.BottomRight,
                _ => LevelPlayBannerPosition.BottomCenter
            };
            m_Banner = new LevelPlayBannerAd(AdConfig.BannerAdUnitId,
                new LevelPlayBannerAd.Config.Builder().SetPosition(position).SetDisplayOnLoad(true).Build());
        }
        private void LoadRewardedInternal()
        {
            if (!m_Initialized || m_RewardedLoading || m_Rewarded.IsAdReady()) return;
            m_RewardedLoading = true;
            try { m_Rewarded.LoadAd(); } catch (Exception exception) { m_RewardedLoading = false; FailRequestInternal(exception.Message); }
        }
        private void LoadInterstitialInternal()
        {
            if (!m_Initialized || m_InterstitialLoading || m_Interstitial.IsAdReady()) return;
            m_InterstitialLoading = true;
            try { m_Interstitial.LoadAd(); } catch (Exception exception) { m_InterstitialLoading = false; FailRequestInternal(exception.Message); }
        }
        private void HandleRewardedLoaded(LevelPlayAdInfo info) { m_RewardedLoading = false; if (!m_InterstitialRequest) TryShowInternal(); }
        private void HandleInterstitialLoaded(LevelPlayAdInfo info) { m_InterstitialLoading = false; if (m_InterstitialRequest) TryShowInternal(); }
        private void HandleRewardedLoadFailed(LevelPlayAdError error)
        {
            m_RewardedLoading = false;
            m_RetryAt = Time.realtimeSinceStartup + c_RetryDelay;
            if (m_Pending && !m_InterstitialRequest) FailRequestInternal(error.ToString());
        }
        private void HandleInterstitialLoadFailed(LevelPlayAdError error)
        {
            m_InterstitialLoading = false;
            m_RetryAt = Time.realtimeSinceStartup + c_RetryDelay;
            if (m_Pending && m_InterstitialRequest) FailRequestInternal(error.ToString());
        }
        private void HandleBannerLoadFailed(LevelPlayAdError error) { Debug.LogWarning("Banner: " + error); }
        private bool MatchesInternal(LevelPlayAdInfo info)
        {
            return m_Showing && (m_AdId == null || info == null || info.AdId == m_AdId);
        }
        private void HandleRewardedDisplayed(LevelPlayAdInfo info) { if (m_Showing && !m_InterstitialRequest) m_AdId = info?.AdId; }
        private void HandleInterstitialDisplayed(LevelPlayAdInfo info) { if (m_Showing && m_InterstitialRequest) m_AdId = info?.AdId; }
        private void HandleReward(LevelPlayAdInfo info, LevelPlayReward reward)
        {
            if (m_InterstitialRequest || !MatchesInternal(info) || m_RewardGranted) return;
            m_RewardGranted = true;
            var callback = m_Callback;
            m_Callback = null;
            try { callback?.Invoke(); } catch (Exception exception) { Debug.LogException(exception); }
            try { OnRewardedAdDoneInvoke(); } catch (Exception exception) { Debug.LogException(exception); }
            try
            {
                if (info != null && info.Revenue.HasValue && AnalyticService.Instance != null)
                    AnalyticService.Instance.WatchAds(info.PlacementName, info.Revenue.Value);
                if (info != null)
                    AppsFlyerSDK.AppsFlyer.sendEvent(AFEventName.OnAdWatched,
                        new System.Collections.Generic.Dictionary<string, string>
                        {
                            { "adcountry", info.Country ?? string.Empty },
                            { "adnetwork", info.AdNetwork ?? string.Empty },
                            { "adrevenue", info.Revenue?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }
                        });
            }
            catch (Exception exception) { Debug.LogWarning("Ads analytics: " + exception.Message); }
        }
        private void OnRewardedAdDoneInvoke() { m_OnRewardedAdDone?.Invoke(); }
        private void OnInterstitialAdDoneInvoke() { m_OnInterstitialAdDone?.Invoke(); }
        private void HandleRewardedClosed(LevelPlayAdInfo info)
        {
            if (m_InterstitialRequest || !MatchesInternal(info)) return;
            // SDK reward and close events may arrive in either order. Keep request ownership briefly.
            m_Closed = true;
            m_Deadline = Time.realtimeSinceStartup + c_CloseGrace;
        }
        private void HandleInterstitialClosed(LevelPlayAdInfo info)
        {
            if (!m_InterstitialRequest || !MatchesInternal(info)) return;
            var callback = m_Callback;
            m_Callback = null;
            try { callback?.Invoke(); OnInterstitialAdDoneInvoke(); }
            catch (Exception exception) { Debug.LogException(exception); }
            finally { FinishRequestInternal(); }
        }
        private void HandleRewardedDisplayFailed(LevelPlayAdInfo info, LevelPlayAdError error)
        { if (!m_InterstitialRequest && MatchesInternal(info)) FailRequestInternal(error.ToString()); }
        private void HandleInterstitialDisplayFailed(LevelPlayAdInfo info, LevelPlayAdError error)
        { if (m_InterstitialRequest && MatchesInternal(info)) FailRequestInternal(error.ToString()); }
        private void FailRequestInternal(string message)
        {
            Debug.LogWarning("Ads: " + message);
            FinishRequestInternal();
            m_RewardedLoading = false;
            m_InterstitialLoading = false;
            SetStatusInternal("Ad unavailable. Please try again.", false);
        }
        private void SetStatusInternal(string message, bool loading)
        {
            if (!Application.isPlaying) return;
            if (!loading)
            {
                ClearStatusInternal();
                var popup = CanvasManager.Instance != null
                    ? CanvasManager.Instance.GetPanel<TextPopUpPanel>() : null;
                if (popup != null) popup.ShowText(message);
                else Debug.LogWarning("Ads notification: TextPopUpPanel is unavailable.");
                return;
            }
            if (m_WaitPanel == null && CanvasManager.Instance != null)
                m_WaitPanel = CanvasManager.Instance.GetPanel<AdsWaitPanel>();
            if (m_WaitPanel != null) m_WaitPanel.ShowWaitMessage(message);
        }
        private void ClearStatusInternal()
        {
            if (m_WaitPanel != null) m_WaitPanel.Hide();
        }
        private void FinishRequestInternal()
        {
            m_Pending = false;
            m_Showing = false;
            m_Closed = false;
            m_Callback = null;
            m_RetryAt = Time.realtimeSinceStartup + c_RetryDelay;
        }
        private void OnDisable()
        {
            ClearStatusInternal();
            FinishRequestInternal();
            LevelPlay.OnInitSuccess -= HandleInitSuccess;
            LevelPlay.OnInitFailed -= HandleInitFailure;
            if (m_Rewarded != null)
            {
                m_Rewarded.OnAdLoaded -= HandleRewardedLoaded;
                m_Rewarded.OnAdLoadFailed -= HandleRewardedLoadFailed;
                m_Rewarded.OnAdDisplayed -= HandleRewardedDisplayed;
                m_Rewarded.OnAdRewarded -= HandleReward;
                m_Rewarded.OnAdClosed -= HandleRewardedClosed;
                m_Rewarded.OnAdDisplayFailed -= HandleRewardedDisplayFailed;
                m_Rewarded.DestroyAd();
            }
            if (m_Interstitial != null)
            {
                m_Interstitial.OnAdLoaded -= HandleInterstitialLoaded;
                m_Interstitial.OnAdLoadFailed -= HandleInterstitialLoadFailed;
                m_Interstitial.OnAdDisplayed -= HandleInterstitialDisplayed;
                m_Interstitial.OnAdClosed -= HandleInterstitialClosed;
                m_Interstitial.OnAdDisplayFailed -= HandleInterstitialDisplayFailed;
                m_Interstitial.DestroyAd();
            }
            if (m_Banner != null) { m_Banner.OnAdLoadFailed -= HandleBannerLoadFailed; m_Banner.DestroyAd(); }
            m_Rewarded = null; m_Interstitial = null; m_Banner = null;
            m_Initialized = false; m_InitRequested = false;
            m_RewardedLoading = false; m_InterstitialLoading = false;
        }
    }
}

// Compatibility for the unused sample; production service no longer inherits it.
public partial class LevelPlaySample
{
    protected virtual void OnRewardedAdDoneInvoke(LevelPlayAdInfo info, LevelPlayReward reward) { }
}

