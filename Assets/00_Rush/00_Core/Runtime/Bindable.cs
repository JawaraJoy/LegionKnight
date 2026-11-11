using UnityEngine;

namespace Rush
{
    public class Bindable : MonoBehaviour, IBindable
    {
        [SerializeField]
        protected MonoBehaviour[] m_Binds;
        private T GetBindInternal<T>()
        {
            foreach (MonoBehaviour bind in m_Binds)
            {
                if (bind is T result)
                {
                    return result;
                }
            }
            return default;
        }
        public bool HasBind<T>(out T found)
        {
            return HasBindInternal(out found);
        }
            
        protected bool HasBindInternal<T>(out T found)
        {
            found = GetBindInternal<T>();
            return found != null;
        }
    }

    public interface IBindable
    {
        public bool HasBind<T>(out T found);
    }
}
