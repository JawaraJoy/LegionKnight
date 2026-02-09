using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class ModuleContext
    {
        [SerializeField, MMReadOnly]
        private Unit m_UnitOwner;
        [SerializeField, MMReadOnly]
        private GameObject m_Module;
        public GameObject Module => m_Module;
        public Unit UnitOwner => m_UnitOwner;
        public bool Initialized => m_UnitOwner != null && m_Module != null;
        public ModuleContext(Unit unitOwner, GameObject module)
        {
            m_UnitOwner = unitOwner;
            m_Module = module;
        }
    }
}
