using UnityEngine;
using UnityEngine.Advertisements;

namespace LegionKnight
{
    public partial class UnityAds : MonoBehaviour, IUnityAdsInitializationListener
    {
        [SerializeField]
        private UnityAdSetting m_AdSetting;
        private string m_GameAdId;
        private bool GetTestMode(MobileDevice device)
        {
            return m_AdSetting.GetTestMode(device);
        }
        private string GetAdGameId(MobileDevice divice)
        {
            return m_AdSetting.GetAdGameId(divice);
        }
        private string GetInterstitialID(MobileDevice divice)
        {
            return m_AdSetting.GetInterstitialID(divice);
        }
        private string GetBannerID(MobileDevice divice)
        {
            return m_AdSetting.GetBannerID(divice);
        }
        public void OnInitializationComplete()
        {
            throw new System.NotImplementedException();
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            throw new System.NotImplementedException();
        }
    }
}
