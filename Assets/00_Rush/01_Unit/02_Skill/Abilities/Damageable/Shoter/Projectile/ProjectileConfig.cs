using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "Rush/Combat/Ammo/Projectile", order = 0)]
    public class ProjectileConfig : AmmoConfig
    {
        [Header("Hit Setup")]
        [SerializeField]
        [Tooltip("If true, projectile despawns immediately on hit")]
        protected bool m_DespawnOnHit = true;
        [SerializeField]
        private ExplodeSetupField m_ExplodeSetUp;
        public bool DespawnOnHit => m_DespawnOnHit;
        public ExplodeSetupField ExplodeSetup => m_ExplodeSetUp;
    }
}
