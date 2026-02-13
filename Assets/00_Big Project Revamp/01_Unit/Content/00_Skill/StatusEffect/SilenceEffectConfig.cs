using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Silence", menuName = "Rush/Combat/StatusEff/Silence", order = 0)]
    public class SilenceEffectConfig : StatusEffectConfig
    {
        [SerializeField]
        private SkillConfig[] m_SpecificSkillsToSilence;
        [SerializeField]
        private SkillCategoryConfig[] m_CategoriesSkillToSilence;
        public override void ApplyEffect(Unit unitTarget)
        {
            if (unitTarget.HasBind(out SkillController skill))
            {
                SilenceBySpecific(true, skill);
                SilenceByCategory(true, skill);
            }
        }

        private void SilenceBySpecific(bool silence, SkillController skill)
        {
            if (m_SpecificSkillsToSilence.Length <= 0) return;
            foreach (SkillConfig config in m_SpecificSkillsToSilence)
            {
                if (skill.HasSkillActivator(config, out Skill activator))
                {
                    if (silence)
                    {
                        activator.EnterSilence();
                    }
                    else
                    {
                        activator.ExitSilence();
                    }
                }
            }
        }
        private void SilenceByCategory(bool silence, SkillController skill)
        {
            List<Skill> activators = new List<Skill>(skill.GetSkillsByMultiCategory(m_CategoriesSkillToSilence));
            foreach(Skill activator in activators)
            {
                if (silence)
                {
                    activator.EnterSilence();
                }
                else
                {
                    activator.ExitSilence();
                }
            }
        }

        public override void DoneEffect(Unit unit)
        {
            if (unit.HasBind(out SkillController skill))
            {
                SilenceBySpecific(true, skill);
                SilenceByCategory(true, skill);
            }
        }
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
