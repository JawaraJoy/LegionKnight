using UnityEngine;
//using Google.Play.AppUpdate;
//using Google.Play.Common;

namespace LegionKnight
{
    /*public class PlayStoreUpdateChecker : MonoBehaviour
    {
        private AppUpdateManager _appUpdateManager;

        void Start()
        {
            _appUpdateManager = new AppUpdateManager();

            // Check for update
            var appUpdateInfoAsync = _appUpdateManager.GetAppUpdateInfo();
            appUpdateInfoAsync.RegisterSuccessCallback(OnUpdateCheck);
        }

        private void OnUpdateCheck(AppUpdateInfo appUpdateInfo)
        {
            if (appUpdateInfo.UpdateAvailability == UpdateAvailability.UpdateAvailable
                && appUpdateInfo.IsUpdateTypeAllowed(AppUpdateType.Immediate))
            {
                Debug.Log("Update available! Starting update...");
                StartImmediateUpdate(appUpdateInfo);
            }
            else
            {
                Debug.Log("No update available.");
            }
        }

        private void StartImmediateUpdate(AppUpdateInfo appUpdateInfo)
        {
            var startUpdateRequest = _appUpdateManager.StartUpdate(
                appUpdateInfo,
                AppUpdateOptions.ImmediateAppUpdateOptions());

            startUpdateRequest.RegisterSuccessCallback(() =>
            {
                Debug.Log("Update started successfully.");
            });
            startUpdateRequest.RegisterFailedCallback(error =>
            {
                Debug.LogError("Update failed: " + error);
            });
        }
    }*/
}
