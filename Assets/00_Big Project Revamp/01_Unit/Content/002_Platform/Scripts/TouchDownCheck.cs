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
                bool isPerfectLanding = PlatformUtility.IsPerfectLanding(this, platform, RushGameManager.Instance.PlatformManager.GlobalPerfectTouchRange);
                m_TouchDown.SetIsStayPerfect(isPerfectLanding, platform.SkillContext);
                RushGameManager.Instance.PlatformManager.TouchDownCheckField.SetIsStayPerfect(isPerfectLanding, platform.SkillContext);
                Vector2 contactPoint = platform.TouchDownSpot.position;
                RushGameManager.Instance.PlatformManager.SetLastContactPoint(contactPoint);
            }
        }
        public void SetPerfectTouchRange(float value)
        {
            m_PerfectTouchRange = Mathf.Clamp(value, 0.1f, 1f);
        }
    }
}
