using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class PlatformSkillField
    {
        [SerializeField]
        private SkillActivatorConfig[] m_OnNormalTouchAbilities;
        [SerializeField]
        private SkillActivatorConfig[] m_OnPerfectTouchAbilities;
        public SkillActivatorConfig[] OnNormalTouchSkill => m_OnNormalTouchAbilities;
        public SkillActivatorConfig[] OnPerfectTouchSkill => m_OnPerfectTouchAbilities;

        public void Activate(Skill skillOwner)
        {
            
        }
    }
}
