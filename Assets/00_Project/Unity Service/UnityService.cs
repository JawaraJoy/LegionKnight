using Unity.Services.Core;
using UnityEngine;

namespace LegionKnight
{
    public partial class UnityService : Singleton<UnityService>
    {
        private void Start()
        {
            UnityServices.InitializeAsync().ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log("Unity Services initialized successfully.");
                }
                else
                {
                    Debug.LogError("Failed to initialize Unity Services: " + task.Exception);
                }
            });
        }
    }
}
