using UnityEngine;

namespace Rush
{
    public abstract partial class UnitConfig
    {
        [SerializeField]
        private SkillConfig[] m_Skills;
        public SkillConfig[] Skills => m_Skills;
    }
}
