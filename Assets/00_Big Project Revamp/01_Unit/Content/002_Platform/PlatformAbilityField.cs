using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class PlatformAbilityField
    {
        [SerializeField]
        private SkillActivatorConfig[] m_OnNormalTouchAbilities;
        [SerializeField]
        private SkillActivatorConfig[] m_OnPerfectTouchAbilities;
        public SkillActivatorConfig[] OnNormalTouchAbilities => m_OnNormalTouchAbilities;
        public SkillActivatorConfig[] OnPerfectTouchAbilities => m_OnPerfectTouchAbilities;

        public void Activate(Skill skillOwner)
        {
            
        }
    }
}
