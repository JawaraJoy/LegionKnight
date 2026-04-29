using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class CastingSliderView : SliderView
    {
        [SerializeField]
        private InteruptSliderview m_InteruptSliderview;
        private RogueLikeCardPanel m_CardPanel;
        private RogueLikeCardPanel CardPanel
        {
            get
            {
                if (m_CardPanel == null)
                {
                    m_CardPanel = CanvasManager.Instance.GetPanel<RogueLikeCardPanel>();
                }
                return m_CardPanel;
            }
        }
        private PausePanel m_PausePanel;
        private PausePanel PausePanel
        {
            get
            {
                if (m_PausePanel == null)
                {
                    m_PausePanel = CanvasManager.Instance.GetPanel<PausePanel>();
                }
                return m_PausePanel;
            }
        }
        private void Start()
        {
            CardPanel.OnShow.AddListener(UnregisterCasting);
            CardPanel.OnHide.AddListener(RegisterCasting);

            PausePanel.OnShow.AddListener(UnregisterCasting);
            PausePanel.OnHide.AddListener(RegisterCasting);
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
            bool canInterupt = skill.SkillConfig.Casting.MaxInterruptCount > 0;
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
