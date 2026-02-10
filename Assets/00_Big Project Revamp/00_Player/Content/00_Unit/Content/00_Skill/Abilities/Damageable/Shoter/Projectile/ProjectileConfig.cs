using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "Rush/Combat/Projectile", order = 0)]
    public class ProjectileConfig : Configuration
    {
        [SerializeField]
        private Projectile m_ProjectilePrefab;
        public Projectile ProjectilePrefab => m_ProjectilePrefab;
        [Header("Hit Setup")]
        [SerializeField]
        [Tooltip("If true, projectile despawns immediately on hit")]
        private bool m_DespawnOnHit = true;
        [SerializeField]
        private ExplodeSetupField m_ExplodeSetUp;
        public ExplodeSetupField ExplodeSetup => m_ExplodeSetUp;
    }
}
