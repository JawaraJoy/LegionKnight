using UnityEngine;

namespace Rush
{
    public class Bindable : MonoBehaviour
    {
        [SerializeField]
        protected MonoBehaviour[] m_Binds;
        private T GetBindInternal<T>() where T : MonoBehaviour
        {
            foreach (MonoBehaviour t in m_Binds)
            {
                if (t.TryGetComponent(out T result))
                {
                    return result;
                }
            }
            return null;
        }
        public bool HasBind<T>(out T found) where T : MonoBehaviour
        {
            found = GetBindInternal<T>();
            return found != null;
        }
    }
}
