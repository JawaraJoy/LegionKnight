using UnityEngine;

namespace Rush
{
    public abstract partial class UnitConfig
    {
        [SerializeField]
        private SkillActivatorConfig[] m_Skills;
        public SkillActivatorConfig[] Skills => m_Skills;
    }
}
