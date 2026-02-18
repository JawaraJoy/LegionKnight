using UnityEngine;

namespace Rush
{
    [RequireComponent(typeof(Collider2D))]
    public class TouchDownCheck : Bindable
    {
        [SerializeField, Range(0.1f, 1f)]
        private float m_PerfectTouchRange = 0.3f;
        [SerializeField]
        private TouchDownCheckField m_TouchDown;
        public TouchDownCheckField TouchDown => m_TouchDown;
        public float PerfectTouchRange => m_PerfectTouchRange;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Platform2D platform))
            {
                PlatformManager platformManager = RushGameManager.Instance.PlatformManager;
                float globalPerfectTouchRate = platformManager.GlobalPerfectTouchRange;
                bool isPerfectLanding = PlatformUtility.IsPerfectLanding(this, platform, globalPerfectTouchRate);
                m_TouchDown.SetIsStayPerfect(isPerfectLanding, platform.SkillContext);
                Vector2 contactPoint = platform.TouchDownSpot.position;

                platform.TouchDownCheck.SetIsStayPerfect(isPerfectLanding, platform.SkillContext);

                platformManager.TouchDownCheckField.SetIsStayPerfect(isPerfectLanding, platform.SkillContext);
                platformManager.SetLastContactPoint(contactPoint);
            }
        }
        public void SetPerfectTouchRange(float value)
        {
            m_PerfectTouchRange = Mathf.Clamp(value, 0.1f, 1f);
        }
    }
}
