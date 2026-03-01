using UnityEngine;
using LegionKnight;

namespace Rush
{
    public class DamageableView : UIView
    {
        [SerializeField]
        private HealthSliderView m_HealthSliderView;
        [SerializeField]
        private ShieldSliderView m_ShieldSliderView;
        public HealthSliderView HealthSliderView => m_HealthSliderView;
        public ShieldSliderView ShieldSliderView => m_ShieldSliderView;
    }
}
