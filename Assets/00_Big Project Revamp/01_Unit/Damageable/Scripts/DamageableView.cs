using UnityEngine;
using LegionKnight;

namespace Rush
{
    public class DamageableView : UIView
    {
        [SerializeField]
        private HealthSliderView m_HealthSliderView;
        public HealthSliderView HealthSliderView => m_HealthSliderView;
    }
}
