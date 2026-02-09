using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class UnitSkill 
    {
        
    }

    public abstract partial class UnitConfig
    {
        [SerializeField]
        private SkillActivatorConfig[] m_Skills;
        public SkillActivatorConfig[] Skills => m_Skills;
    }
}
