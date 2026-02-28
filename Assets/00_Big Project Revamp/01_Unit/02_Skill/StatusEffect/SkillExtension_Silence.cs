using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class SkillExtension_Silence
    {
        
    }
    public partial class Skill
    {

        [SerializeField, MMReadOnly]
        private SkillActivationState m_PreSilenceState = SkillActivationState.Idle;
        public void EnterSilence()
        {
            m_PreSilenceState = m_State;

            // Silence HARUS membatalkan casting
            if (m_State == SkillActivationState.Casting)
            {
                FailCasting();
            }

            ChangeState(SkillActivationState.Silenced);
            Debug.Log($"Skill {name} entered silence. Previous state: {m_PreSilenceState}");
        }
        public void ExitSilence()
        {
            if (m_State == SkillActivationState.Silenced)
            {
                ChangeState(m_PreSilenceState == SkillActivationState.Silenced ? SkillActivationState.Idle : m_PreSilenceState);
            }
        }
    }
}
