using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class BossHealthView : View
    {
        [SerializeField]
        private Canvas m_Cam;
        [SerializeField]
        private Slider m_HealthBar;

        
        public void SetHealthBar(float set)
        {
            if (m_Cam.worldCamera == null)
            {
                m_Cam.worldCamera = Camera.main;
            }
            m_HealthBar.value = set;
        }
    }
}
