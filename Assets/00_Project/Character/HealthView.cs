using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class HealthView : UIView
    {
        [SerializeField]
        private Canvas m_Cam;
        [SerializeField]
        private Slider m_HealthBar;

        [SerializeField]
        private UnityEvent<float> m_OnHealthRateChanged = new();
        public void SetHealthBar(float set)
        {
            if (m_Cam.worldCamera == null)
            {
                m_Cam.worldCamera = Camera.main;
            }
            m_HealthBar.value = set;
            m_OnHealthRateChanged.Invoke(set);
        }
    }
}
