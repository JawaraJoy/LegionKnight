
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public partial class DirectHeal : AbilityDeliver
    {
        [SerializeField]
        private Healer m_HealerPrefab;
        public Healer HealerPrefab => m_HealerPrefab;

        private List<Healer> m_ActiveHealer = new();
        public List<Healer> ActiveHealer => m_ActiveHealer;
        private Queue<Healer> m_HealerPool = new();

        private DirectHealAbilityConfig m_HealConfig;
        public DirectHealAbilityConfig HealConfig => m_HealConfig;

        public override void Init(AbilityConfig config, ISkillContext context)
        {
            base.Init(config, context);

            if (m_AbilityConfig is DirectHealAbilityConfig healConfig)
            {
                m_HealConfig = healConfig;
            }

            PreWarm();
        }

        public override void Activate()
        {
            List<ITargetable> targets = new(GetTargetsInternal());

            StopAllCoroutines();
            StartCoroutine(HealRoutine(targets));

            base.Activate();
        }

        private IEnumerator HealRoutine(List<ITargetable> targets)
        {
            var setup = m_HealConfig.SpawningSetup;

            int fireCount = setup.FireCount;
            FireMode mode = setup.FireMode;

            float interval = setup.FireInterval;
            int burstCount = setup.BurstCount;
            float burstInterval = setup.BurstInterval;

            int dir = 1;
            int shapeIndex = 0;

            switch (mode)
            {
                case FireMode.Instant:
                    for (int i = 0; i < fireCount; i++)
                        SpawnSingle(i, fireCount, targets);
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

                default:
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
            int count = m_HealConfig.SpawningSetup.FireCount;

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

            TargetDistributeMode mode = m_HealConfig.TargetDistributeMode;

            switch (mode)
            {
                case TargetDistributeMode.SameTarget:
                    return targets[0];

                case TargetDistributeMode.RandomPerLaunch:
                    return targets[Random.Range(0, targets.Count)];

                default:
                    return targets[shotIndex % targets.Count];
            }
        }

        private void SpawnSingle(int index, int totalCount, List<ITargetable> targets)
        {
            Healer healer = GetFromPool();

            ITargetable target = ResolveTarget(index, targets);
            if (target == null)
            {
                ReturnToPool(healer);
                return;
            }

            healer.transform.position = target.TargetTransform.position;

            healer.Heal(target, m_HealConfig.InitialDelay);

            m_ActiveHealer.Add(healer);
        }

        private void PreWarm()
        {
            if (m_HealConfig == null || m_HealerPrefab == null)
                return;

            int count = m_HealConfig.SpawningSetup.PreWarmCount;

            for (int i = 0; i < count; i++)
            {
                Healer healer = CreateNewHealer();
                ReturnToPool(healer);
            }
        }

        private Healer CreateNewHealer()
        {
            Healer healer = Instantiate(m_HealerPrefab, m_DeliverTransform);
            healer.gameObject.SetActive(false);
            healer.OnHealDone.AddListener((ctx) => ReturnToPool(healer));
            return healer;
        }

        private Healer GetFromPool()
        {
            Healer healer;

            if (m_HealerPool.Count > 0)
                healer = m_HealerPool.Dequeue();
            else
                healer = CreateNewHealer();

            healer.transform.SetParent(null);
            healer.gameObject.SetActive(true);
            healer.Init(m_AbilityContext);
            return healer;
        }

        private void ReturnToPool(Healer healer)
        {
            if (healer == null)
                return;

            RushGameManager.Instance.StartCoroutine(ReturningPool(healer, 1f));
        }

        private IEnumerator ReturningPool(Healer healer, float wait)
        {
            yield return new WaitForSeconds(wait);
            healer.gameObject.SetActive(false);
            healer.transform.SetParent(m_DeliverTransform);

            m_ActiveHealer.Remove(healer);
            m_HealerPool.Enqueue(healer);
        }
    }
}
