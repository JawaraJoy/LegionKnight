using UnityEngine;
using LegionKnight;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Tools;

namespace Rush
{
    public class SliderView : UIView, IUpdater
    {
        [SerializeField]
        protected Slider m_Slider;
        [SerializeField]
        protected TextMeshProUGUI m_ValueText;

        [SerializeField, MMReadOnly]
        protected float m_Rate;

        protected float m_CurrentDuration;
        protected float m_MaxDuration;
        public bool IsActive => IsShowInternal;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }
        protected override void HideInternal()
        {
            base.HideInternal();
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }
        public virtual void SetSlider(int current, int max)
        {
            m_ValueText.text = $"{current}/{max}";
            m_Rate = (float)current / max;
            m_Slider.value = m_Rate;
        }
        public virtual void SetDuration(float value)
        {
            if (!IsHasDuration(value))
            {
                HideInternal();
            }
            else
            {
                ShowInternal();
                m_MaxDuration = value;
                m_CurrentDuration = value;
            }
        }
        protected virtual void ReduceDuration(float value)
        {
            m_CurrentDuration -= value;
            if (!IsHasDuration(m_CurrentDuration))
            {
                HideInternal();
            }
            else
            {
                m_ValueText.text = GetSliderText(m_CurrentDuration, m_MaxDuration);
                m_Rate = m_CurrentDuration / m_MaxDuration;
                m_Slider.value = m_Rate;
            }
        }
        protected virtual string GetSliderText(float current, float max)
        {
            return $"{current:0.#}/{max:0.#}";
        }
        protected virtual bool IsHasDuration(float val)
        {
            return val > 0;
        }

        public virtual void Tick()
        {
            // reduce current by delta time
            ReduceDuration(Time.deltaTime);
        }
    }
}
