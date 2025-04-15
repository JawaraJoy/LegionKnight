using UnityEngine;

namespace LegionKnight
{
    public enum MobileDevice
    {
        Android = 0,
        IOS = 1,
    }
    [System.Serializable]
    public partial class AdPlatformSetting
    {
        [SerializeField]
        private MobileDevice m_MobileDevice = MobileDevice.Android;
        [SerializeField]
        private bool m_TestMode = false;
        [SerializeField]
        private string m_AdGameID;
        [SerializeField]
        private string m_InterstitialID = "Interstitial_Android";
        [SerializeField]
        private string m_RewardedID = "Rewarded_Android";
        [SerializeField]
        private string m_BanerID = "Banner_Android";

        public MobileDevice MobileDevice => m_MobileDevice;
        public bool TestMode => m_TestMode;
        public string AdGameID => m_AdGameID;
        public string InterstitialID => m_InterstitialID;
        public string BannerID => m_BanerID;
        public string RewardedID => m_RewardedID;
    }
}
