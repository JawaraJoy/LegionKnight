using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Silence", menuName = "Rush/Combat/StatusEff/Silence", order = 0)]
    public class SilenceEffectConfig : StatusEffectConfig
    {
        [SerializeField]
        private SkillActivatorConfig[] m_SpecificSkillsToSilence;
        [SerializeField]
        private SkillCategoryConfig[] m_CategoriesSkillToSilence;
        public override void ApplyEffect(Unit unitTarget)
        {
            if (unitTarget.HasBind(out Skill skill))
            {
                SilenceBySpecific(true, skill);
                SilenceByCategory(true, skill);
            }
        }

        private void SilenceBySpecific(bool silence, Skill skill)
        {
            if (m_SpecificSkillsToSilence.Length <= 0) return;
            foreach (SkillActivatorConfig config in m_SpecificSkillsToSilence)
            {
                if (skill.HasSkillActivator(config, out SkillActivator activator))
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
        private void SilenceByCategory(bool silence, Skill skill)
        {
            List<SkillActivator> activators = new List<SkillActivator>(skill.GetSkillsByMultiCategory(m_CategoriesSkillToSilence));
            foreach(SkillActivator activator in activators)
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
            if (unit.HasBind(out Skill skill))
            {
                SilenceBySpecific(true, skill);
                SilenceByCategory(true, skill);
            }
        }
    }
    public partial class SkillActivator
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
