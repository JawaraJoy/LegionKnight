
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Shot Ability", menuName = "Rush/Combat/Ability/Shot")]
    public class ShotAbilityConfig : DamageAbilityConfig
    {
        [SerializeField]
        private SpawnSetupField m_SpawningSetup;
        public SpawnSetupField SpawningSetup => m_SpawningSetup;
        
        [SerializeField]
        private SpawnShapeConfig m_SpawnShape;

        [Header("Hit Setup")]
        [SerializeField]
        [Tooltip("If true, projectile despawns immediately on hit")]
        private bool m_DespawnOnHit = true;
        [SerializeField]
        private ExplodeSetupField m_ExplodeSetUp;
        public ExplodeSetupField ExplodeSetup => m_ExplodeSetUp;

        [Header("Movement & Targeting")]
        [SerializeField]
        private bool m_ShoterLookAtTargetOnActivate;
        [SerializeField]
        private ProjectileTargetingMode m_ProjectileTargetingMode = ProjectileTargetingMode.None;
        [SerializeField]
        private TargetDistributeMode m_TargetDistributeMode;
        [SerializeField]
        private float m_ProjectileHomingTurnSpeed = 90f;
        [SerializeField]
        [Tooltip("Movement speed in units per second")]
        private float m_ProjectileSpeed = 10f;
        [SerializeField]
        [Tooltip("How long the projectile stays alive in seconds (0 = infinite)")]
        private float m_ProjectileLifetime = 0f;
        [SerializeField]
        [Tooltip("Maximum distance the projectile can travel (0 = infinite)")]
        private float m_MaxDistance = 10f;
        public TargetDistributeMode TargetDistributeMode => m_TargetDistributeMode;
        public bool ShoterLookAtTargetOnActivate => m_ShoterLookAtTargetOnActivate;
        public ProjectileTargetingMode ProjectileTargetingMode => m_ProjectileTargetingMode;
        public float ProjectileHomingTurnSpeed => m_ProjectileHomingTurnSpeed;
        public SpawnShapeConfig SpawnShape => m_SpawnShape;
        public float ProjectileSpeed => m_ProjectileSpeed;
        public float ProjectileLifetime => m_ProjectileLifetime;
        public float MaxDistance => m_MaxDistance;
        public bool DespawnOnHit => m_DespawnOnHit;
        

        protected override float GetDamage(AbilityContext context)
        {
            return AbilityUltility.GetFinalEffectAmount(context);
        }
    }

    public enum ProjectileTargetingMode
    {
        None = 0,
        Facing = 1,
        Homing = 2,
    }
    
}
