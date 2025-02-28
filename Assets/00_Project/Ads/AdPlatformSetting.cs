using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public partial class AdPlatformSetting
    {
        [SerializeField]
        private string m_PlatformName = "Android";
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

        public string PlatformName => m_PlatformName;
        public bool TestMode => m_TestMode;
        public string AdGameID => m_AdGameID;
        public string InterstitialID => m_InterstitialID;
        public string BannerID => m_BanerID;
    }
}
