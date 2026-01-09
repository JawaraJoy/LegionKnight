using MoreMountains.Tools;
using UnityEngine;
using static Rush.Projectile;

namespace Rush
{
    [CreateAssetMenu(fileName = "Shot Ability", menuName = "Rush/Combat/Ability/Shot")]
    public class ShotAbilityConfig : DamageAbilityConfig
    {
        [Header("Spawning Setup")]
        [SerializeField]
        private int m_PreWarmCount = 5;
        [SerializeField]
        private FireMode m_FireMode;
        [SerializeField]
        private int m_FireCount = 5;
        [SerializeField]
        private float m_FireInterval = 0.2f;
        [SerializeField] 
        private int m_BurstCount = 3;
        [SerializeField] 
        private float m_BurstInterval = 0.3f;
        
        [SerializeField]
        private SpawnShapeConfig m_SpawnShape;

        [Header("Hit Setup")]
        [SerializeField]
        [Tooltip("If true, projectile despawns immediately on hit")]
        private bool m_DespawnOnHit = true;
        [SerializeField]
        private bool m_ExplodeOnDespawn = false;
        [SerializeField]
        private float m_ExplosionRadius = 5f;

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

        public FireMode FireMode => m_FireMode;
        public TargetDistributeMode TargetDistributeMode => m_TargetDistributeMode;
        public int BurstCount => m_BurstCount;
        public float BurstInterval => m_BurstInterval;
        public bool ShoterLookAtTargetOnActivate => m_ShoterLookAtTargetOnActivate;
        public ProjectileTargetingMode ProjectileTargetingMode => m_ProjectileTargetingMode;
        public float ProjectileHomingTurnSpeed => m_ProjectileHomingTurnSpeed;
        public int PreWarmCount => m_PreWarmCount;
        public int FireCount => m_FireCount;
        public float FireInterval => m_FireInterval;
        public SpawnShapeConfig SpawnShape => m_SpawnShape;
        public float ProjectileSpeed => m_ProjectileSpeed;
        public float ProjectileLifetime => m_ProjectileLifetime;
        public float MaxDistance => m_MaxDistance;
        public bool DespawnOnHit => m_DespawnOnHit;
        public bool ExplodeOnDespawn => m_ExplodeOnDespawn;
        public float ExplosionRadius => m_ExplosionRadius;

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
    public enum FireMode
    {
        Instant,     // semua keluar sekaligus
        Burst,       // keluar per kelompok
        Gatling,     // satu-satu cepat (interval tetap)
        Loop,        // arah muter 0→N→0
        PingPong,    // arah bolak-balik
        Random       // arah random tiap shot
    }
}
