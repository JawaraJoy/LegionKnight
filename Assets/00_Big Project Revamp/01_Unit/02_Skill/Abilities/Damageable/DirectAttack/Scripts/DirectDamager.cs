using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public partial class DirectDamager : DamageAbilityDeliver
    {
        [SerializeField]
        private Attacker m_AttackerPrefab;
        public Attacker AttackerPrefab => m_AttackerPrefab;
        [SerializeField, MMReadOnly, Tooltip("List of currently active projectiles in the scene.")]
        private List<Attacker> m_ActiveAttacker = new();
        public List<Attacker> ActiveAttacker => m_ActiveAttacker;

        [SerializeField, MMReadOnly, Tooltip("Queue of inactive projectiles ready to be reused.")]
        private Queue<Attacker> m_AttackerPool = new();
        [SerializeField, MMReadOnly, Tooltip("Cached shot ability configuration used by this shooter.")]
        private DirectDamageAbilityConfig m_DirectDamageAbilityConfig;

        public override void Init(AbilityConfig config, ISkillContext context)
        {
            base.Init(config, context);
            if (m_AbilityConfig is DirectDamageAbilityConfig directDamageAbilityConfig)
            {
                m_DirectDamageAbilityConfig = directDamageAbilityConfig;
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
            var setup = m_DirectDamageAbilityConfig.SpawningSetup;

            int fireCount = m_AbilityConfig.UseAllTargetsInRange ? Mathf.Min(targets.Count, setup.FireCount) : setup.FireCount;
            FireMode mode = setup.FireMode;

            float interval = setup.FireInterval;
            int burstCount = Mathf.Min(setup.BurstCount, fireCount);
            float burstInterval = setup.BurstInterval;

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

                            yield return new WaitForSeconds(interval);
                        }

                        yield return new WaitForSeconds(burstInterval);
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
            int count = m_DirectDamageAbilityConfig.SpawningSetup.FireCount;

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
                    return shotIndex;
            }
        }
        private ITargetable ResolveTarget(int shotIndex, List<ITargetable> targets)
        {
            if (targets == null || targets.Count == 0)
                return null;

            TargetDistributeMode mode = m_DirectDamageAbilityConfig.TargetDistributeMode;

            switch (mode)
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
        private void SpawnSingle(int index, int totalCount, List<ITargetable> targets)
        {
            Attacker attacker = GetFromPool();

            ITargetable target = ResolveTarget(index, targets);
            if (target == null)
            {
                ReturnToPool(attacker);
                return;
            }

            if (!target.ModuleContext.Unit.HasBind(out Damageable damageable))
            {
                ReturnToPool(attacker);
                return;
            }

            // posisi attacker (kalau ada VFX)
            attacker.transform.position = target.TargetTransform.position;

            // trigger attack
            attacker.DirectAttack(target, m_DirectDamageAbilityConfig.AttackDelay);

            m_ActiveAttacker.Add(attacker);
        }
        private void PreWarm()
        {
            if (m_DirectDamageAbilityConfig == null || m_AttackerPrefab == null)
                return;

            int count = m_DirectDamageAbilityConfig.SpawningSetup.PreWarmCount;

            for (int i = 0; i < count; i++)
            {
                Attacker attacker = CreateNewAttacker();
                ReturnToPool(attacker);
            }
        }
        /// <summary>
        /// Instantiates a new projectile, initializes it with ability context,
        /// and keeps it inactive for pooling.
        /// </summary>
        private Attacker CreateNewAttacker()
        {
            Attacker attacker = Instantiate(m_AttackerPrefab, m_DeliverTransform);
            attacker.gameObject.SetActive(false);
            attacker.OnAttackDone.AddListener((context) => ReturnToPool(attacker));
            return attacker;
        }

        /// <summary>
        /// Retrieves a projectile from pool if available,
        /// otherwise creates a new one.
        /// </summary>
        private Attacker GetFromPool()
        {
            Attacker attacker;

            if (m_AttackerPool.Count > 0)
            {
                attacker = m_AttackerPool.Dequeue();
            }
            else
            {
                attacker = CreateNewAttacker();
            }

            attacker.transform.SetParent(null);
            attacker.gameObject.SetActive(true);
            attacker.Init(m_AbilityContext);

            return attacker;
        }

        private void ReturnToPool(Attacker attacker)
        {
            if (attacker == null)
                return;

            //projectile.OnDespawned();
            attacker.gameObject.SetActive(false);
            attacker.transform.SetParent(m_DeliverTransform);

            if (m_ActiveAttacker.Contains(attacker))
            {
                m_ActiveAttacker.Remove(attacker);
            }

            m_AttackerPool.Enqueue(attacker);
        }
        public void NotifyAttackerDone(Attacker attacker)
        {
            ReturnToPool(attacker);
        }
    }
}
