using UnityEngine;

namespace Rush
{
    public class InteruptSliderview : SliderView
    {
        private Skill m_Source;
        public void StartInterupt(Skill skill)
        {
            DetachInternal();
            if (skill == null || skill.MaxInterruptCount <= 0) { HideInternal(); return; }
            m_Source = skill;
            m_Source.OnCastingInterruptEvent.AddListener(UpdateDamageInternal);
            ShowInternal();
            UpdateDamageInternal(skill.CurrentInterruptCount,skill.MaxInterruptCount);
        }
        private void UpdateDamageInternal(int damage, int max)
        {
            if (max > 0) SetSliderInternal(damage,max);
        }
        private void DetachInternal()
        {
            if (m_Source != null) m_Source.OnCastingInterruptEvent.RemoveListener(UpdateDamageInternal);
            m_Source = null;
        }
        protected override void HideInternal() { DetachInternal(); base.HideInternal(); }
        private void OnDisable() => DetachInternal();
    }
}
