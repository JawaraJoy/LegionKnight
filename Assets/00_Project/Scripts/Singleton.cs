using UnityEngine;

namespace LegionKnight
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T m_Instance;
        private static readonly object m_Lock = new object();
        private static bool m_ApplicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (m_ApplicationIsQuitting)
                {
                    Debug.LogWarning($"[Singleton] Instance of '{typeof(T)}' already destroyed. Returning null.");
                    return null;
                }

                lock (m_Lock)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = FindFirstObjectByType<T>();

                        if (m_Instance == null)
                        {
                            GameObject singletonObject = new GameObject($"{typeof(T)} (Singleton)");
                            m_Instance = singletonObject.AddComponent<T>();
                            singletonObject.AddComponent<DontDestroy>(); // Add persistence here
                        }
                    }

                    return m_Instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (m_Instance == null)
            {
                m_Instance = this as T;
            }
            else if (m_Instance != this)
            {
                Destroy(gameObject); // Prevent duplicates
            }
        }

        protected virtual void OnDestroy()
        {
            if (m_Instance == this)
            {
                m_ApplicationIsQuitting = true;
            }
        }
    }
}
