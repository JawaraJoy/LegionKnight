using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class CastingSliderView : SliderView
    {
        [SerializeField]
        private InteruptSliderview m_InteruptSliderview;

        private void OnEnable()
        {
            RegisterCasting();
        }

        private void RegisterCasting()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }
        private void UnregisterCasting()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }
        public void StartCasting(Skill skill)
        {
            Debug.Log($"UIStartCasting: {skill?.SkillConfig.name}");
            skill.OnCastingFailEvent.RemoveListener(() => EndCastingInternal(skill));
            skill.OnCastingSuccessEvent.RemoveListener(() => EndCastingInternal(skill));

            //skill.OnCastingUpdateEvent.AddListener((progress) => SetCastingDuration(skill));
            SetDurationInternal(skill.SkillConfig.Casting.CastDuration);

            skill.OnCastingFailEvent.AddListener(() => EndCastingInternal(skill));
            skill.OnCastingSuccessEvent.AddListener(() => EndCastingInternal(skill));
            bool canInterupt = skill.SkillConfig.Casting.MaxInteruptResist > 0;
            if (canInterupt)
            {
                m_InteruptSliderview.StartInterupt(skill);
            }
            else
            {
                m_InteruptSliderview.Hide();
            }
            ShowInternal();
        }
        protected override void HideInternal()
        {
            base.HideInternal();
            m_InteruptSliderview.Hide();
        }
        private void SetCastingDuration(Skill skill)
        {
            int duration = Mathf.RoundToInt(skill.RemainingCastTime);
            int maxDuration = Mathf.RoundToInt(skill.SkillConfig.Casting.CastDuration);
            SetSliderInternal(duration, maxDuration);
        }
        public void EndCasting(Skill skill)
        {
            EndCastingInternal(skill);
        }
        private void EndCastingInternal(Skill skill)
        {
            skill.OnCastingUpdateEvent.RemoveListener((progress) => SetCastingDuration(skill));
            HideInternal();
        }
    }
}
