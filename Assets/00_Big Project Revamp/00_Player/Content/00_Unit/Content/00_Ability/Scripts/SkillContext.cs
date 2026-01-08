using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class SkillContext
    {
        [SerializeField]
        private SkillActivator m_Activator;
        [SerializeField, MMReadOnly]
        private ModuleContext m_ModuleContext;
        public SkillActivator Activator => m_Activator;
        public ModuleContext ModuleContext => m_ModuleContext;
        public bool Initialized => m_ModuleContext.Initialized && m_Activator != null;
        public SkillContext(SkillActivator activator, ModuleContext moduleContext)
        {
            m_Activator = activator;
            m_ModuleContext = moduleContext;
        }
    }
}
