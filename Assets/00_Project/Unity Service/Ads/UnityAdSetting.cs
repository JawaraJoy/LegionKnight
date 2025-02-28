using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Ad Setting", menuName = "Legion Knight/Ad Setting")]
    public partial class UnityAdSetting : ScriptableObject
    {
        [SerializeField]
        private List<AdPlatformSetting> m_PlatformSettings = new();

        private AdPlatformSetting GetAdPlatformSetting(MobileDevice device)
        {
            AdPlatformSetting match = m_PlatformSettings.Find(x => x.MobileDevice == device);
            return match;
        }
        public bool GetTestMode(MobileDevice device)
        {
            return GetAdPlatformSetting(device).TestMode;
        }
        public string GetAdGameId(MobileDevice divice)
        {
            return GetAdPlatformSetting(divice).AdGameID;
        }
        public string GetInterstitialID(MobileDevice divice)
        {
            return GetAdPlatformSetting(divice).InterstitialID;
        }
        public string GetBannerID(MobileDevice divice)
        {
            return GetAdPlatformSetting(divice).BannerID;
        }

    }
}
