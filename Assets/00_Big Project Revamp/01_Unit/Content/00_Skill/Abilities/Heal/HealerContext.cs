using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class HealerContext 
    {
        [SerializeField, MMReadOnly]
        private Healer m_Healer;
        private readonly IDamageable m_Damageable;
        public Healer Healer => m_Healer;
        public IDamageable Damageable => m_Damageable;
        public HealerContext(Healer healer, IDamageable damageable)
        {
            m_Healer = healer;
            m_Damageable = damageable;
        }
    }
}
