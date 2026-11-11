#if UNITY_EDITOR
namespace Rush { public sealed class LiuraValidationAnalytics : global::BaseTenjin {
public override void Init(System.String p0) {  }
public override void InitWithSharedSecret(System.String p0, System.String p1) {  }
public override void InitWithAppSubversion(System.String p0, System.Int32 p1) {  }
public override void InitWithSharedSecretAppSubversion(System.String p0, System.String p1, System.Int32 p2) {  }
public override void Connect() {  }
public override void Connect(System.String p0) {  }
public override void OptIn() {  }
public override void OptOut() {  }
public override void OptInParams(System.Collections.Generic.List<System.String> p0) {  }
public override void OptOutParams(System.Collections.Generic.List<System.String> p0) {  }
public override System.Boolean OptInOutUsingCMP() { return default(System.Boolean); }
public override void OptInGoogleDMA() {  }
public override void OptOutGoogleDMA() {  }
public override void AppendAppSubversion(System.Int32 p0) {  }
public override void SendEvent(System.String p0) {  }
public override void SendEvent(System.String p0, System.String p1) {  }
public override void Transaction(System.String p0, System.String p1, System.Int32 p2, System.Double p3, System.String p4, System.String p5, System.String p6) {  }
public override void TransactionAmazon(System.String p0, System.String p1, System.Int32 p2, System.Double p3, System.String p4, System.String p5) {  }
public override void Subscription(System.String p0, System.String p1, System.Double p2, System.String p3, System.String p4, System.String p5, System.String p6, System.String p7, System.String p8, System.String p9) {  }
public override void SubscriptionWithStoreKit(System.String p0, System.String p1, System.Double p2) {  }
public override void GetDeeplink(Tenjin.DeferredDeeplinkDelegate p0) {  }
public override void GetAttributionInfo(Tenjin.AttributionInfoDelegate p0) {  }
public override void RegisterAppForAdNetworkAttribution() {  }
public override void UpdateConversionValue(System.Int32 p0) {  }
public override void UpdatePostbackConversionValue(System.Int32 p0) {  }
public override void UpdatePostbackConversionValue(System.Int32 p0, System.String p1) {  }
public override void UpdatePostbackConversionValue(System.Int32 p0, System.String p1, System.Boolean p2) {  }
public override void RequestTrackingAuthorizationWithCompletionHandler(System.Action<System.Int32> p0) {  }
public override void DebugLogs() {  }
public override void SetAppStoreType(AppStoreType p0) {  }
public override void SubscribeAppLovinImpressions() {  }
public override void AppLovinImpressionFromJSON(System.String p0) {  }
public override void SubscribeIronSourceImpressions() {  }
public override void IronSourceImpressionFromJSON(System.String p0) {  }
public override void SubscribeLevelPlayRewardedAdImpressions(System.Object p0) {  }
public override void SubscribeLevelPlayInterstitialAdImpressions(System.Object p0) {  }
public override void SubscribeLevelPlayBannerAdImpressions(System.Object p0) {  }
public override void SubscribeHyperBidImpressions() {  }
public override void HyperBidImpressionFromJSON(System.String p0) {  }
public override void SubscribeAdMobBannerViewImpressions(System.Object p0, System.String p1) {  }
public override void SubscribeAdMobRewardedAdImpressions(System.Object p0, System.String p1) {  }
public override void SubscribeAdMobInterstitialAdImpressions(System.Object p0, System.String p1) {  }
public override void SubscribeAdMobRewardedInterstitialAdImpressions(System.Object p0, System.String p1) {  }
public override void AdMobImpressionFromJSON(System.String p0) {  }
public override void SubscribeTopOnImpressions() {  }
public override void TopOnImpressionFromJSON(System.String p0) {  }
public override void SubscribeCASImpressions(System.Object p0) {  }
public override void SubscribeCASBannerImpressions(System.Object p0) {  }
public override void CASImpressionFromJSON(System.String p0) {  }
public override void SubscribeTradPlusImpressions() {  }
public override void TradPlusImpressionFromJSON(System.String p0) {  }
public override void TradPlusImpressionFromAdInfo(System.Collections.Generic.Dictionary<System.String,System.Object> p0) {  }
public override void SubscribeCloudXImpressions() {  }
public override void CloudXImpressionFromJSON(System.String p0) {  }
public override void CustomImpressionFromJSON(System.String p0) {  }
public override void HandleOpenUrl(System.String p0) {  }
public override void SetCustomerUserId(System.String p0) {  }
public override System.String GetCustomerUserId() { return default(System.String); }
public override void SetSessionTime(System.Int32 p0) {  }
public override void SetCacheEventSetting(System.Boolean p0) {  }
public override void SetEncryptRequestsSetting(System.Boolean p0) {  }
public override System.String GetAnalyticsInstallationId() { return default(System.String); }
public override void SetGoogleDMAParameters(System.Boolean p0, System.Boolean p1) {  }
public override System.Collections.Generic.Dictionary<System.String,System.String> GetUserProfileDictionary() { return default(System.Collections.Generic.Dictionary<System.String,System.String>); }
public override void ResetUserProfile() {  }
} }
#endif
