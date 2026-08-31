#if UNITY_ANDROID
using Google.Play.AppUpdate;
#endif
using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class InAppUpdateAvailable : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_UpdateInfoText;
#if UNITY_ANDROID
        public void SetAvailableInfo(AppUpdateInfo info)
        {
            string version = info.AvailableVersionCode.ToString();
            m_UpdateInfoText.text = $"[AppUpdateInfo] Update to version {version} First to Continue";
        }
#endif
        public void Quit()
        {
            Application.Quit();
        }
    }
}
