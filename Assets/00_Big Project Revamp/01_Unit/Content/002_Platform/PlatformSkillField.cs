using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class PlatformSkillField
    {
        [SerializeField]
        private SkillConfig[] m_OnNormalTouchAbilities;
        [SerializeField]
        private SkillConfig[] m_OnPerfectTouchAbilities;
        public SkillConfig[] OnNormalTouchSkill => m_OnNormalTouchAbilities;
        public SkillConfig[] OnPerfectTouchSkill => m_OnPerfectTouchAbilities;
    }
}
