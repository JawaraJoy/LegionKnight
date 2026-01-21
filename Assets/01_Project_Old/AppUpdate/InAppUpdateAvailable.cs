using Google.Play.AppUpdate;
using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class InAppUpdateAvailable : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_UpdateInfoText;
        public void SetAvailableInfo(AppUpdateInfo info)
        {
            string version = info.AvailableVersionCode.ToString();
            m_UpdateInfoText.text = $"[AppUpdateInfo] Update to version {version} First to Continue";
        }
        public void Quit()
        {
            Application.Quit();
        }
    }
}
