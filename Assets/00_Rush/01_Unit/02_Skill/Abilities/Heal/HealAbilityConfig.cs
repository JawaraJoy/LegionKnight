using UnityEngine;

namespace Rush
{
    public partial class HealAbilityConfig : AbilityConfig
    {
        [SerializeField]
        private int m_HealTickCount = 0;
        [SerializeField]
        private float m_HealTickInterval = 0;
        public int HealTickCount => m_HealTickCount;
        public float HealTickInterval => m_HealTickInterval;
    }
}
