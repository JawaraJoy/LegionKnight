#if UNITY_ANDROID
using UnityEngine;

namespace LegionKnight
{
    public class InAppUpdateManagerAgent : MonoBehaviour
    {
        private InAppUpdateManager m_Manager;

        private InAppUpdateManager GetManager()
        {
            if (m_Manager == null)
            {
                m_Manager = UnityService.Instance.InAppUpdateManager;
            }
            return m_Manager;
        }
        public void CheckUpdate()
        {
            GetManager().CheckUpdate();
        }
        public void StartUpdate()
        {
            GetManager().StartUpdate();
        }
    }
}
#endif
