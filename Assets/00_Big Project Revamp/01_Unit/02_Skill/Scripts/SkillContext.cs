
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class SkillContext : ISkillContext
    {
        private readonly ISkill m_Skill;
        private readonly IModuleContext m_ModuleContext;
        public ISkill Skill => m_Skill;
        public IModuleContext ModuleContext => m_ModuleContext;
        public bool Initialized => m_ModuleContext.Initialized && m_Skill != null;
        public SkillContext(ISkill skill, IModuleContext moduleContext)
        {
            m_Skill = skill;
            m_ModuleContext = moduleContext;
        }
    }
}
