using Rush;
using UnityEngine;

namespace Rush
{
    public class UnitSkill { }

    public partial class UnitConfig
    {
        [SerializeField]
        private SkillConfig[] m_Skills;
        public SkillConfig[] Skills => m_Skills;
    }
}
