
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class ModuleContext : IModuleContext
    {
        private Unit m_Unit;
        private GameObject m_Module;
        public GameObject Module => m_Module;
        public Unit Unit => m_Unit;
        public bool Initialized => m_Unit != null && m_Module != null;
        public ModuleContext(Unit unitOwner, GameObject module)
        {
            m_Unit = unitOwner;
            m_Module = module;
        }
    }
}
