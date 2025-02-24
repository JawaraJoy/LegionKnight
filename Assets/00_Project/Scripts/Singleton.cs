using UnityEngine;

namespace LegionKnight
{
    public partial class Singleton<T> : MonoBehaviour where T : MonoBehaviour
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
                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
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
                            DontDestroyOnLoad(singletonObject);
                        }
                    }

                    return m_Instance;
                }
            }
        }

        protected virtual void OnDestroy()
        {
            m_ApplicationIsQuitting = true;
        }
    }
}
