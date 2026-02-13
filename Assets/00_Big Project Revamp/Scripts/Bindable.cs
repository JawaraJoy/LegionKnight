using UnityEngine;

namespace Rush
{
    public class Bindable : MonoBehaviour, IBindable
    {
        [SerializeField]
        protected MonoBehaviour[] m_Binds;
        private T GetBindInternal<T>() where T : MonoBehaviour
        {
            foreach (MonoBehaviour bind in m_Binds)
            {
                if (bind is T result)
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

    public interface IBindable
    {
        public bool HasBind<T>(out T found) where T : MonoBehaviour;
    }
}
