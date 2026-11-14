using UnityEngine;

namespace Rush
{
    public class CastingSliderView : SliderView
    {
        [SerializeField] private InteruptSliderview m_InteruptSliderview;
        private Skill m_Source;

        // SkillController already forwards cast start/end to the prefab. Read the same
        // clock as the HUD instead of registering anonymous callbacks on every cast.
        public void StartCasting(Skill skill)
        {
            if (skill == null || skill.SkillConfig == null) return;
            m_Source = skill;
            ShowInternal();
            if (m_InteruptSliderview != null)
            {
                if (skill.MaxInterruptCount > 0) m_InteruptSliderview.StartInterupt(skill);
                else m_InteruptSliderview.Hide();
            }
            Tick();
        }
        public override void Tick()
        {
            if (m_Source == null || !m_Source.IsCasting) { HideInternal(); return; }
            float max = m_Source.SkillConfig.Casting.CastDuration;
            m_Rate = max > 0 ? Mathf.Clamp01(m_Source.RemainingCastTime / max) : 0;
            m_Slider.value = m_Rate;
            m_ValueText.SetText("{0:1}s",Mathf.Max(0,m_Source.RemainingCastTime));
        }
        public void EndCasting(Skill skill) { if (skill == m_Source) HideInternal(); }
        protected override void HideInternal()
        {
            m_Source = null;
            if (m_InteruptSliderview != null) m_InteruptSliderview.Hide();
            base.HideInternal();
        }
    }
}
