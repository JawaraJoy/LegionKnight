using Firebase;
using UnityEngine;

namespace Rush
{
    public class FirebaseAnalytic : MonoBehaviour
    {
        public async void Init()
        {
            var status = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (status == DependencyStatus.Available)
            {
                Debug.Log("Firebase Ready");
            }
            else
            {
                Debug.LogError(status);
            }
        }
    }
}
