using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class SkillContext : ISkillContext
    {
        [SerializeField]
        private Skill m_Skill;
        [SerializeField, MMReadOnly]
        private ModuleContext m_ModuleContext;
        public Skill Skill => m_Skill;
        public IModuleContext ModuleContext => m_ModuleContext;
        public bool Initialized => m_ModuleContext.Initialized && m_Skill != null;
        public SkillContext(Skill skill, ModuleContext moduleContext)
        {
            m_Skill = skill;
            m_ModuleContext = moduleContext;
        }
    }
}
