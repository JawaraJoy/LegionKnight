using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class HealerContext 
    {
        [SerializeField, MMReadOnly]
        private Healer m_Healer;
        [SerializeField, MMReadOnly]
        private Damageable m_Damageable;
        public Healer Healer => m_Healer;
        public Damageable Damageable => m_Damageable;
        public HealerContext(Healer healer, Damageable damageable)
        {
            m_Healer = healer;
            m_Damageable = damageable;
        }
    }
}
