using UnityEngine;

namespace Rush
{
    public class Bindable : MonoBehaviour
    {
        [SerializeField]
        protected MonoBehaviour[] m_Binds;
        private T GetBindInternal<T>() where T : MonoBehaviour
        {
            T found = null;
            foreach (MonoBehaviour t in m_Binds)
            {
                if (t.TryGetComponent(out T find))
                {
                    found = find;
                }
                else
                {
                    Debug.LogError($"No Bind with Type of {find.GetType()}");
                }
            }
            return found;
        }
        public bool HasBind<T>(out T found) where T : MonoBehaviour
        {
            bool isFound = GetBindInternal<T>();
            if (isFound)
            {
                found = GetBindInternal<T>();
            }
            else
            {
                found = null;
            }
            return isFound;
        }
    }
}
