using UnityEngine;

namespace Rush
{
    [RequireComponent(typeof(Collider2D))]
    public class TouchDownCheck : Bindable
    {
        [SerializeField, Range(0f, 1f)]
        private float m_PerfectTouchRange = 0.3f;
        [SerializeField]
        private TouchDownCheckField m_TouchDown;
        public TouchDownCheckField TouchDown => m_TouchDown;
        public float PerfectTouchRange => m_PerfectTouchRange;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Platform2D platform))
            {
                PlatformHandler platformManager = RushGameManager.Instance.StageManager.PlatformHandler;
                float globalPerfectTouchRate = platformManager.GlobalPerfectTouchRange;
                bool isPerfectLanding = PlatformUtility.IsPerfectLanding(this, platform, globalPerfectTouchRate);

                m_TouchDown.SetIsStayPerfect(isPerfectLanding, platform.SkillContext);
                Vector2 contactPoint = platform.TouchDownSpot.position;

                platform.TouchDownCheck.SetIsStayPerfect(isPerfectLanding, platform.SkillContext);
                platform.OnReachDestinationInvoke();

                platformManager.TouchDownCheckField.SetIsStayPerfect(isPerfectLanding, platform.SkillContext);
                platformManager.SetLastContactPoint(contactPoint);

                
            }
            Debug.Log($"TouchDown{collision.name}");
        }
        public void SetPerfectTouchRange(float value)
        {
            m_PerfectTouchRange = Mathf.Clamp(value, 0.1f, 1f);
        }
    }
}
