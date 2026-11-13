using UnityEngine;

namespace Rush
{
    public class NormalPlatformThrusterView : MonoBehaviour
    {
        [SerializeField]
        private Platform2D m_Platform;
        [SerializeField]
        private GameObject m_ThrustContent;

        public bool IsThrustActive => m_ThrustContent != null && m_ThrustContent.activeSelf;

        private void OnEnable()
        {
            if (m_Platform == null)
            {
                SetThrustActiveInternal(false);
                return;
            }

            m_Platform.OnBoostStart?.AddListener(OnFlightStartedInternal);
            m_Platform.OnBoostEnd?.AddListener(OnFlightEndedInternal);
            SetThrustActiveInternal(m_Platform.IsBoosting);
        }

        private void OnDisable()
        {
            if (m_Platform != null)
            {
                m_Platform.OnBoostStart?.RemoveListener(OnFlightStartedInternal);
                m_Platform.OnBoostEnd?.RemoveListener(OnFlightEndedInternal);
            }
            SetThrustActiveInternal(false);
        }

        private void OnFlightStartedInternal(float duration, int comboCount)
        {
            SetThrustActiveInternal(m_Platform != null && m_Platform.IsBoosting);
        }

        private void OnFlightEndedInternal()
        {
            SetThrustActiveInternal(false);
        }

        private void SetThrustActiveInternal(bool active)
        {
            if (m_ThrustContent != null && m_ThrustContent.activeSelf != active)
                m_ThrustContent.SetActive(active);
        }
    }
}
