#if UNITY_ANDROID
using UnityEngine;

namespace LegionKnight
{
    public class InAppUpdateManager : InAppUpdate
    {
        
    }

    public partial class UnityService
    {
        [SerializeField]
        private InAppUpdateManager m_InAppUpdate;

        public InAppUpdateManager InAppUpdateManager => m_InAppUpdate;
    }
}
#endif
