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
    public class Shooter : DamageAbilityDeliver
    {
        [Header("Projectile")]
        [SerializeField]
        private AmmoConfig m_AmmoConfig;

        [Header("Runtime (Read Only)")]
        [SerializeField, MMReadOnly]
        private List<Ammo> m_ActiveProjectiles = new();
        public List<Ammo> ActiveProjectiles => m_ActiveProjectiles;

        [SerializeField, MMReadOnly]
        private Queue<Ammo> m_ProjectilePool = new();

        [SerializeField, MMReadOnly]
        private ShooterAbilityConfig m_ShooterAbilityConfig;
        public ShooterAbilityConfig ShooterAbilityConfig => m_ShooterAbilityConfig;

        public bool Initialized => m_AbilityContext.Initialized;

        public override void Init(AbilityConfig config, ISkillContext context)
        {
            base.Init(config, context);

            if (config is ShooterAbilityConfig shooterConfig)
            {
                m_ShooterAbilityConfig = shooterConfig;
            }

            PreWarm();
        }

        public override void Activate()
        {
            List<ITargetable> targets = new(GetTargetsInternal());

            if (m_ShooterAbilityConfig.ShoterLookAtTargetOnActivate)
                LookAtTargetInternal(targets);

            StopAllCoroutines();
            StartCoroutine(AttackRoutine(targets));

            base.Activate();
        }

        private void LookAtTargetInternal(List<ITargetable> targets)
        {
            if (targets == null || targets.Count == 0) return;

            ITargetable target = targets[0];
            if (target?.TargetTransform == null) return;

            Vector2 dir = target.TargetTransform.position - m_DeliverTransform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            m_DeliverTransform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private IEnumerator AttackRoutine(List<ITargetable> targets)
        {
            var setup = m_ShooterAbilityConfig.SpawningSetup;

            int fireCount = setup.FireCount;
            FireMode mode = setup.FireMode;

            float fireInterval = setup.FireInterval;
            int burstCount = setup.BurstCount;
            float burstInterval = setup.BurstInterval;

            int direction = 1;
            int shapeIndex = 0;

            switch (mode)
            {
                case FireMode.Instant:
                    for (int i = 0; i < fireCount; i++)
                    {
                        SpawnSingle(i, i, fireCount, targets);
                    }
                    yield break;

                case FireMode.Burst:
                    int fired = 0;
                    while (fired < fireCount)
                    {
                        for (int i = 0; i < burstCount && fired < fireCount; i++)
                        {
                            // shapeIndex untuk posisi/bentuk spawn
                            // fired sebagai targetIndex supaya distribusi target merata
                            int resolvedShapeIndex = ResolveShapeIndex(mode, i, fireCount, ref shapeIndex, ref direction);
                            SpawnSingle(resolvedShapeIndex, fired, fireCount, targets);
                            fired++;
                            yield return new WaitForSeconds(fireInterval);
                        }

                        yield return new WaitForSeconds(burstInterval);
                    }
                    break;

                default: // Interval / Loop / PingPong / Random
                    for (int i = 0; i < fireCount; i++)
                    {
                        int resolvedShapeIndex = ResolveShapeIndex(mode, i, fireCount, ref shapeIndex, ref direction);
                        SpawnSingle(resolvedShapeIndex, i, fireCount, targets);
                        yield return new WaitForSeconds(fireInterval);
                    }
                    break;
            }
        }

        private int ResolveShapeIndex(FireMode mode, int shotIndex, int totalCount, ref int shapeIndex, ref int direction)
        {
            if (totalCount <= 0)
                return 0;

            switch (mode)
            {
                case FireMode.Random:
                    return Random.Range(0, totalCount);

                case FireMode.Loop:
                    shapeIndex = (shapeIndex + 1) % totalCount;
                    return shapeIndex;

                case FireMode.PingPong:
                    if (totalCount == 1)
                        return 0;

                    shapeIndex += direction;

                    if (shapeIndex >= totalCount - 1 || shapeIndex <= 0)
                        direction *= -1;

                    return shapeIndex;

                default:
                    return shotIndex;
            }
        }

        /// <param name="shapeIndex">Index untuk menentukan posisi/bentuk spawn dari SpawnShape</param>
        /// <param name="targetIndex">Index urutan tembakan global, dipakai untuk distribusi target</param>
        /// <param name="totalCount">Total projectile yang ditembakkan</param>
        /// <param name="targets">List target yang tersedia</param>
        protected virtual void SpawnSingle(int shapeIndex, int targetIndex, int totalCount, List<ITargetable> targets)
        {
            Ammo ammo = GetFromPool();

            m_DeliverTransform.GetPositionAndRotation(out Vector3 pos, out Quaternion rot);

            SpawnShapeConfig shape = m_ShooterAbilityConfig.SpawnShape;
            if (shape != null)
            {
                shape.GetSpawnTransform(m_DeliverTransform, shapeIndex, totalCount, out pos, out rot);
            }

            ammo.transform.SetPositionAndRotation(pos, rot);

            ITargetable target = ResolveTarget(targetIndex, targets);

            ammo.Shot(target);

            if (!m_ActiveProjectiles.Contains(ammo))
                m_ActiveProjectiles.Add(ammo);
        }

        private ITargetable ResolveTarget(int targetIndex, List<ITargetable> targets)
        {
            if (targets == null || targets.Count == 0)
                return null;

            switch (m_ShooterAbilityConfig.TargetDistributeMode)
            {
                case TargetDistributeMode.SameTarget:
                    return targets[0];

                case TargetDistributeMode.RandomPerLaunch:
                    return targets[Random.Range(0, targets.Count)];

                case TargetDistributeMode.SplitTargets:
                default:
                    return targets[targetIndex % targets.Count];
            }
        }

        public ITargetable GetNewTargetForAmmo()
        {
            List<ITargetable> targets = new(GetTargetsInternal());
            if (targets == null || targets.Count == 0)
                return null;

            return targets[0];
        }

        private void PreWarm()
        {
            if (m_ShooterAbilityConfig == null || m_AmmoConfig == null)
                return;

            int count = m_ShooterAbilityConfig.SpawningSetup.PreWarmCount;

            for (int i = 0; i < count; i++)
            {
                Ammo ammo = CreateNewAmmo();
                ReturnToPool(ammo);
            }
        }

        private Ammo CreateNewAmmo()
        {
            Ammo ammo = Instantiate(m_AmmoConfig.AmmoPrefab, m_DeliverTransform);
            ammo.gameObject.SetActive(false);
            ammo.Init(m_AbilityContext, m_AmmoConfig);
            return ammo;
        }

        private Ammo GetFromPool()
        {
            Ammo ammo = m_ProjectilePool.Count > 0
                ? m_ProjectilePool.Dequeue()
                : CreateNewAmmo();

            ammo.transform.SetParent(null);
            ammo.gameObject.SetActive(true);
            ammo.OnSpawnFromPool();
            return ammo;
        }

        private void ReturnToPool(Ammo ammo)
        {
            if (ammo == null)
                return;

            ammo.gameObject.SetActive(false);
            ammo.transform.SetParent(m_DeliverTransform);

            m_ActiveProjectiles.Remove(ammo);
            m_ProjectilePool.Enqueue(ammo);
        }

        public void NotifyProjectileFinished(Ammo ammo)
        {
            ReturnToPool(ammo);
        }
    }
}