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

            StopAllCoroutines();
            StartCoroutine(AttackRoutine(targets));

            base.Activate();
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
                        SpawnSingle(i, fireCount, targets);
                    }
                    yield break;

                case FireMode.Burst:
                    int fired = 0;
                    while (fired < fireCount)
                    {
                        for (int i = 0; i < burstCount && fired < fireCount; i++)
                        {
                            int index = ResolveShapeIndex(mode, fired, ref shapeIndex, ref direction);
                            SpawnSingle(index, fireCount, targets);
                            fired++;
                            yield return new WaitForSeconds(fireInterval);
                        }

                        yield return new WaitForSeconds(burstInterval);
                    }
                    break;

                default: // Interval / Loop / PingPong / Random
                    for (int i = 0; i < fireCount; i++)
                    {
                        int index = ResolveShapeIndex(mode, i, ref shapeIndex, ref direction);
                        SpawnSingle(index, fireCount, targets);
                        yield return new WaitForSeconds(fireInterval);
                    }
                    break;
            }
        }

        private int ResolveShapeIndex(FireMode mode, int shotIndex, ref int shapeIndex, ref int direction)
        {
            int count = m_ShooterAbilityConfig.SpawningSetup.FireCount;

            switch (mode)
            {
                case FireMode.Random:
                    return Random.Range(0, count);

                case FireMode.Loop:
                    shapeIndex = (shapeIndex + 1) % count;
                    return shapeIndex;

                case FireMode.PingPong:
                    shapeIndex += direction;
                    if (shapeIndex >= count - 1 || shapeIndex <= 0)
                        direction *= -1;
                    return shapeIndex;

                default:
                    return shotIndex;
            }
        }

        protected virtual void SpawnSingle(int index, int totalCount, List<ITargetable> targets)
        {
            Ammo ammo = GetFromPool();

            m_DeliverTransform.GetPositionAndRotation(out Vector3 pos, out Quaternion rot);

            SpawnShapeConfig shape = m_ShooterAbilityConfig.SpawnShape;
            if (shape != null)
            {
                shape.GetSpawnTransform(m_DeliverTransform, index, totalCount, out pos, out rot);
            }

            ammo.transform.SetPositionAndRotation(pos, rot);

            ITargetable target = ResolveTarget(index, targets);
            FaceSpawnToTarget(target);

            ammo.Shot(target);

            if (!m_ActiveProjectiles.Contains(ammo))
                m_ActiveProjectiles.Add(ammo);
        }

        private ITargetable ResolveTarget(int shotIndex, List<ITargetable> targets)
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
                    return targets[shotIndex % targets.Count];
            }
        }

        public ITargetable GetNewTargetForAmmo()
        {
            List<ITargetable> targets = new(GetTargetsInternal());
            if (targets == null || targets.Count == 0)
                return null;

            return targets[0];
        }

        private void FaceSpawnToTarget(ITargetable targetable)
        {
            if (!m_ShooterAbilityConfig.ShoterLookAtTargetOnActivate)
                return;

            AbilityUltility.LookAtFirstTarget2D(m_DeliverTransform, targetable);
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
            ammo.Init(m_AbilityContext, m_AmmoConfig);

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
