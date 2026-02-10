using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class PlatformAbilityField
    {
        [SerializeField]
        private PlatformAbilityTriggerState m_TriggerState = PlatformAbilityTriggerState.OnBad;
        [SerializeField]
        private SkillActivatorConfig m_SkillToActive;
        public PlatformAbilityTriggerState TriggerState => m_TriggerState;
        public SkillActivatorConfig SkillToActive => m_SkillToActive;

        public void Activate(Skill skillOwner)
        {

        }
    }
}
