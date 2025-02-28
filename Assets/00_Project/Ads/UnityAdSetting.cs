using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Ad Setting", menuName = "Legion Knight/Ad Setting")]
    public partial class UnityAdSetting : ScriptableObject
    {
        [SerializeField]
        private List<AdPlatformSetting> m_PlatformSettings = new();

        private AdPlatformSetting GetAdPlatformSetting(string platformName)
        {
            AdPlatformSetting match = m_PlatformSettings.Find(x => x.PlatformName == platformName);
            return match;
        }
        public string GetPlatformName(string platformName)
        {
            return GetAdPlatformSetting(platformName).PlatformName;
        }
        public bool GetTestMode(string platformName)
        {
            return GetAdPlatformSetting(platformName).TestMode;
        }
        public string GetAdGameId(string platformName)
        {
            return GetAdPlatformSetting(platformName).AdGameID;
        }
        public string GetInterstitialID(string platformName)
        {
            return GetAdPlatformSetting(platformName).InterstitialID;
        }
        public string GetBannerID(string platformName)
        {
            return GetAdPlatformSetting(platformName).BannerID;
        }

    }
}
