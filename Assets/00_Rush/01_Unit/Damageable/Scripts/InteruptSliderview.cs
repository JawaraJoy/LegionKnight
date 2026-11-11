using UnityEngine;

namespace Rush
{
    public class InteruptSliderview : SliderView
    {
        protected override void ShowInternal()
        {
            base.ShowInternal();
            SetSliderInternal(0, 1);
        }
        public void StartInterupt(Skill skill)
        {
            if (skill == null) return;

            skill.OnCastingInterruptEvent.RemoveListener((current, max) => SetCastingDuration(skill));
            skill.OnCastingInterruptEvent.AddListener((current, max) => SetCastingDuration(skill));
            ShowInternal();
        }

        private void SetCastingDuration(Skill skill)
        {
            int current = skill.CurrentInterruptCount;
            int max = skill.MaxInterruptCount;
            SetSliderInternal(current, max);
        }
    }
}
