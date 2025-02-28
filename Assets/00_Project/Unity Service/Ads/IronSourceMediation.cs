using UnityEngine;

namespace LegionKnight
{
    public partial class IronSourceMediation : MonoBehaviour
    {
        [SerializeField]
        private IronSourceMediationSettings m_Settings;
#if UNITY_ANDROID
        private string AppIDInternal => m_Settings.AndroidAppKey;
#endif
#if UNITY_IOS
        private string AppIDInternal => m_Settings.IOSAppKey;
#endif

        public void Init()
        {
            IronSourceAdQuality.Initialize(AppIDInternal);
        }
    }
}
