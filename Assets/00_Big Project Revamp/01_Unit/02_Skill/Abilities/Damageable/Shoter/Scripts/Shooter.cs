
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

        private List<Ammo> m_ActiveProjectiles = new();
        public List<Ammo> ActiveProjectiles => m_ActiveProjectiles;

        private Queue<Ammo> m_ProjectilePool = new();

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

            if (m_ShooterAbilityConfig != null && m_ShooterAbilityConfig.DeliverLookAtTargetOnActivate)
                LookAtTargetInternal(targets);

            StopAllCoroutines();
            StartCoroutine(AttackRoutine(targets));

            base.Activate();
        }

        

        private IEnumerator AttackRoutine(List<ITargetable> targets)
        {
            if (m_ShooterAbilityConfig == null)
                yield break;

            var setup = m_ShooterAbilityConfig.SpawningSetup;

            int fireCount = m_AbilityConfig.UseAllTargetsInRange
                ? Mathf.Min(targets.Count, setup.FireCount)
                : setup.FireCount;

            FireMode mode = setup.FireMode;
            float fireInterval = setup.FireInterval;
            int burstCount = Mathf.Min(setup.BurstCount, fireCount);
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
                            int resolvedShapeIndex = ResolveShapeIndex(mode, i, fireCount, ref shapeIndex, ref direction);
                            SpawnSingle(resolvedShapeIndex, fired, fireCount, targets);
                            fired++;

                            if (fireInterval > 0f)
                                yield return new WaitForSeconds(fireInterval);
                        }

                        if (fired < fireCount && burstInterval > 0f)
                            yield return new WaitForSeconds(burstInterval);
                    }
                    break;

                default: // Interval / Loop / PingPong / Random
                    for (int i = 0; i < fireCount; i++)
                    {
                        int resolvedShapeIndex = ResolveShapeIndex(mode, i, fireCount, ref shapeIndex, ref direction);
                        SpawnSingle(resolvedShapeIndex, i, fireCount, targets);

                        if (fireInterval > 0f)
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
                    {
                        int current = shapeIndex;
                        shapeIndex = (shapeIndex + 1) % totalCount;
                        return current;
                    }

                case FireMode.PingPong:
                    {
                        if (totalCount == 1)
                            return 0;

                        int current = shapeIndex;

                        shapeIndex += direction;
                        if (shapeIndex >= totalCount - 1 || shapeIndex <= 0)
                            direction *= -1;

                        return current;
                    }

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
            Ammo ammo = GetFromPoolInactive();

            m_DeliverTransform.GetPositionAndRotation(out Vector3 pos, out Quaternion rot);

            SpawnShapeConfig shape = m_ShooterAbilityConfig.SpawnShape;
            if (shape != null)
            {
                shape.GetSpawnTransform(m_DeliverTransform, shapeIndex, totalCount, out pos, out rot);
            }

            ammo.PrepareForSpawn(pos, rot);

            ITargetable target = ResolveTarget(targetIndex, targets);

            
            ammo.gameObject.SetActive(true);
            ammo.transform.localScale = Vector3.one;
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

        /// <summary>
        /// Ambil ammo dari pool dalam kondisi masih inactive.
        /// State reset dan transform placement dilakukan sebelum SetActive(true).
        /// </summary>
        protected virtual Ammo GetFromPoolInactive()
        {
            Ammo ammo = m_ProjectilePool.Count > 0
                ? m_ProjectilePool.Dequeue()
                : CreateNewAmmo();

            ammo.transform.SetParent(null, true);
            return ammo;
        }

        private void ReturnToPool(Ammo ammo)
        {
            if (ammo == null)
                return;

            if (!m_ProjectilePool.Contains(ammo))
            {
                ammo.gameObject.SetActive(false);
                ammo.transform.SetParent(m_DeliverTransform, false);
                ammo.transform.localPosition = Vector3.zero;
                ammo.transform.localRotation = Quaternion.identity;

                m_ActiveProjectiles.Remove(ammo);
                m_ProjectilePool.Enqueue(ammo);
            }
        }

        public void NotifyProjectileFinished(Ammo ammo)
        {
            ReturnToPool(ammo);
        }
    }
}