using UnityEngine;

namespace Rush
{
    [DefaultExecutionOrder(-1000)]
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T s_Instance;
        private static bool s_IsApplicationQuitting;

        public static T Instance
        {
            get
            {
                if (s_IsApplicationQuitting)
                {
                    Debug.LogWarning($"[Singleton] Instance of '{typeof(T)}' requested while the application is quitting.");
                    return null;
                }

                if (s_Instance == null)
                {
                    Debug.LogError($"[Singleton] No registered instance of '{typeof(T)}' is available. Initialize its owner first.");
                }

                return s_Instance;
            }
        }
        protected virtual void Awake()
        {
            s_IsApplicationQuitting = false;
            if (s_Instance != null && s_Instance != this)
            {
                Debug.LogWarning($"[Singleton] Duplicate instance of '{typeof(T)}' destroyed on '{gameObject.name}'.");
                gameObject.SetActive(false);
                Destroy(gameObject);
                return;
            }

            s_Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (s_Instance == this)
            {
                s_Instance = null;
            }
        }
    

        protected virtual void OnApplicationQuit()
        {
            if (s_Instance == this)
            {
                s_IsApplicationQuitting = true;
            }
        }
}
}
