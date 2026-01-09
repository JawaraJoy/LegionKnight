using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    /// <summary>
    /// Ability deliverer that spawns and reuses projectiles using internal pooling.
    /// Responsible for managing projectile lifecycle: spawn, reuse, and return to pool.
    /// </summary>
    public class Shoter : AbilityDeliver
    {
        [Header("Projectile")]

        [SerializeField, Tooltip("Projectile prefab that will be instantiated and reused by this shooter.")]
        private Projectile m_ProjectilePrefab;

        [Header("Runtime (Read Only)")]

        [SerializeField, MMReadOnly, Tooltip("List of currently active projectiles in the scene.")]
        private List<Projectile> m_ActiveProjectiles = new();
        public List<Projectile> ActiveProjectiles => m_ActiveProjectiles;

        [SerializeField, MMReadOnly, Tooltip("Queue of inactive projectiles ready to be reused.")]
        private Queue<Projectile> m_ProjectilePool = new();

        [SerializeField, MMReadOnly, Tooltip("Cached shot ability configuration used by this shooter.")]
        private ShotAbilityConfig m_ShotAbilityConfig;

        /// <summary>
        /// Indicates whether ability context has been initialized.
        /// </summary>
        public bool Initialized => m_AbilityContext.Initialized;
        public ShotAbilityConfig ShotAbilityConfig => m_ShotAbilityConfig;

        /// <summary>
        /// Initializes shooter with ability config and context, and prewarms projectile pool.
        /// </summary>
        public override void Init(AbilityConfig config, SkillContext context)
        {
            base.Init(config, context);

            if (m_Config is ShotAbilityConfig shotAbilityConfig)
            {
                m_ShotAbilityConfig = shotAbilityConfig;
                m_Purpose = AbilityPurpose.Damaging;
            }

            PreWarm();
        }

        /// <summary>
        /// Activates the ability and spawns projectiles toward available targets.
        /// </summary>
        public override void Activate()
        {
            List<Targetable> targets = new(GetTargetsInternal());

            StopAllCoroutines();
            StartCoroutine(FireRoutine(targets));

            if (targets.Count > 0)
                FacingFirstTarget2D(targets[0]);

            base.Activate();
        }
        private IEnumerator FireRoutine(List<Targetable> targets)
        {
            int fireCount = m_ShotAbilityConfig.FireCount;
            FireMode mode = m_ShotAbilityConfig.FireMode;

            float interval = m_ShotAbilityConfig.FireInterval;
            int burstCount = m_ShotAbilityConfig.BurstCount;
            float burstInterval = m_ShotAbilityConfig.BurstInterval;

            int dir = 1;
            int shapeIndex = 0;

            switch (mode)
            {
                case FireMode.Instant:
                    for (int i = 0; i < fireCount; i++)
                    {
                        SpawnSingle(i, fireCount, targets);
                    }
                    yield break;

                case FireMode.Burst:
                    int fired = 0;
                    while (fired < fireCount)
                    {
                        for (int j = 0; j < burstCount && fired < fireCount; j++)
                        {
                            int index = ResolveShapeIndex(mode, fired, ref shapeIndex, ref dir);
                            SpawnSingle(index, fireCount, targets);
                            fired++;

                            yield return new WaitForSeconds(interval); // delay antar peluru
                        }

                        yield return new WaitForSeconds(burstInterval); // delay antar burst
                    }
                    break;

                default: // Gatling, Loop, PingPong, Random
                    for (int i = 0; i < fireCount; i++)
                    {
                        int index = ResolveShapeIndex(mode, i, ref shapeIndex, ref dir);
                        SpawnSingle(index, fireCount, targets);
                        yield return new WaitForSeconds(interval);
                    }
                    break;
            }
        }
        private int ResolveShapeIndex(FireMode mode, int shotIndex, ref int shapeIndex, ref int dir)
        {
            int count = m_ShotAbilityConfig.FireCount;

            switch (mode)
            {
                case FireMode.Random:
                    return Random.Range(0, count);

                case FireMode.Loop:
                    shapeIndex = (shapeIndex + 1) % count;
                    return shapeIndex;

                case FireMode.PingPong:
                    shapeIndex += dir;
                    if (shapeIndex >= count - 1 || shapeIndex <= 0)
                        dir *= -1;
                    return shapeIndex;

                default:
                    return shotIndex; // Instant / Burst / Gatling
            }
        }
        private void SpawnSingle(int index, int totalCount, List<Targetable> targets)
        {
            Projectile projectile = GetFromPool();

            SpawnShapeConfig shape = m_ShotAbilityConfig.SpawnShape;

            Vector3 pos = m_VfxSpawnPost.position;
            Quaternion rot = m_VfxSpawnPost.rotation;

            if (shape != null)
            {
                shape.GetSpawnTransform(m_VfxSpawnPost, index, totalCount, out pos, out rot);
            }

            projectile.transform.SetPositionAndRotation(pos, rot);

            Targetable target = null;
            if (targets != null && targets.Count > 0)
            {
                target = targets[index % targets.Count];
            }

            projectile.Shot(target);

            m_ActiveProjectiles.Add(projectile);
        }


        /// <summary>
        /// Rotates spawn point to face target in 2D (XY plane, Z-axis rotation only).
        /// </summary>
        private void FacingFirstTarget2D(Targetable targetable)
        {
            if (!m_ShotAbilityConfig.ShoterLookAtTargetOnActivate) return;
            Targetable.LookAtFirstTarget2D(m_VfxSpawnPost, targetable);
        }

        /// <summary>
        /// Pre-instantiates projectile instances based on PreWarmCount
        /// and stores them in the inactive pool.
        /// </summary>
        private void PreWarm()
        {
            if (m_ShotAbilityConfig == null || m_ProjectilePrefab == null)
                return;

            int count = m_ShotAbilityConfig.PreWarmCount;

            for (int i = 0; i < count; i++)
            {
                Projectile projectile = CreateNewProjectile();
                ReturnToPool(projectile);
            }
        }

        /// <summary>
        /// Instantiates a new projectile, initializes it with ability context,
        /// and keeps it inactive for pooling.
        /// </summary>
        private Projectile CreateNewProjectile()
        {
            Projectile projectile = Instantiate(m_ProjectilePrefab, m_VfxSpawnPost);
            projectile.gameObject.SetActive(false);
            projectile.Init(m_AbilityContext);
            return projectile;
        }

        /// <summary>
        /// Retrieves a projectile from pool if available,
        /// otherwise creates a new one.
        /// </summary>
        private Projectile GetFromPool()
        {
            Projectile projectile;

            if (m_ProjectilePool.Count > 0)
            {
                projectile = m_ProjectilePool.Dequeue();
            }
            else
            {
                projectile = CreateNewProjectile();
            }

            projectile.transform.SetParent(null);
            projectile.gameObject.SetActive(true);
            projectile.OnSpawned();

            return projectile;
        }

        /// <summary>
        /// Deactivates projectile and returns it to the pool for reuse.
        /// Also removes it from active projectile list.
        /// </summary>
        private void ReturnToPool(Projectile projectile)
        {
            if (projectile == null)
                return;

            projectile.OnDespawned();
            projectile.gameObject.SetActive(false);
            projectile.transform.SetParent(m_VfxSpawnPost);

            if (m_ActiveProjectiles.Contains(projectile))
            {
                m_ActiveProjectiles.Remove(projectile);
            }

            m_ProjectilePool.Enqueue(projectile);
        }

        /// <summary>
        /// Called by projectile when its lifetime ends or it hits something,
        /// so it can be returned back to pool.
        /// </summary>
        public void NotifyProjectileFinished(Projectile projectile)
        {
            ReturnToPool(projectile);
        }
    }
}
