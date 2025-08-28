using UnityEngine;
using AppsFlyerSDK;

namespace LegionKnight
{
    public class AppsflyerManager : Singleton<AppsflyerManager>, IAppsFlyerConversionData
    {
        [Header("Appflyer Settings")]
        [SerializeField] string devKey = "";
        [SerializeField] bool enableDebug = true;

        private static AppsflyerManager _instance;
        public static AppsflyerManager Instance => _instance;

        protected override void Awake()
        {
            base.Awake();

            //enable debug log while testing
            AppsFlyer.setIsDebug(enableDebug);

            AppsFlyer.initSDK(devKey, null, this);
            AppsFlyer.startSDK();

        }

        //conversion & attribution callbacks
        public void onConversionDataSuccess(string conversionData)
        {
            AppsFlyer.AFLog("onConversionDataSuccess", conversionData);
        }

        //conversion & attribution failed callbacks
        public void onConversionDataFail(string error)
        {
            AppsFlyer.AFLog("onConversionDataFailed", error);
        }

        //attribution success callback
        public void onAppOpenAttribution(string attributionData)
        {
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
        }

        //attribution failed callback
        public void onAppOpenAttributionFailure(string error)
        {
            AppsFlyer.AFLog("onAppOpenAttributionFailed", error);
        }
    }
}
