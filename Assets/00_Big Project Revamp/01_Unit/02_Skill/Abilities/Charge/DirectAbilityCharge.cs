using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public partial class DirectAbilityCharge : AbilityDeliver
    {
        [SerializeField]
        private Charger m_ChargePrefab;
        public Charger ChargerPrefab => m_ChargePrefab;

        [SerializeField, MMReadOnly]
        private List<Charger> m_ActiveCharger = new();
        public List<Charger> ActiveCharge => m_ActiveCharger;

        [SerializeField, MMReadOnly]
        private Queue<Charger> m_HealerPool = new();

        [SerializeField, MMReadOnly]
        private DirectChargeAbilityConfig m_ChargeConfig;
        public DirectChargeAbilityConfig ChargeConfig => m_ChargeConfig;

        public override void Init(AbilityConfig config, ISkillContext context)
        {
            base.Init(config, context);

            if (m_AbilityConfig is DirectChargeAbilityConfig chargeConfig)
            {
                m_ChargeConfig = chargeConfig;
            }

            PreWarm();
        }

        public override void Activate()
        {
            List<ITargetable> targets = new(GetTargetsInternal());

            StopAllCoroutines();
            StartCoroutine(ChargeRoutine(targets));

            base.Activate();
        }

        private IEnumerator ChargeRoutine(List<ITargetable> targets)
        {
            var setup = m_ChargeConfig.SpawningSetup;

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
            int count = m_ChargeConfig.SpawningSetup.FireCount;

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

            TargetDistributeMode mode = m_ChargeConfig.TargetDistributeMode;

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
            Charger charger = GetFromPool();

            ITargetable target = ResolveTarget(index, targets);
            if (target == null)
            {
                ReturnToPool(charger);
                return;
            }

            charger.transform.position = target.TargetTransform.position;

            charger.Charge(target, m_ChargeConfig.InitialDelay);

            m_ActiveCharger.Add(charger);
        }

        private void PreWarm()
        {
            if (m_ChargeConfig == null || m_ChargePrefab == null)
                return;

            int count = m_ChargeConfig.SpawningSetup.PreWarmCount;

            for (int i = 0; i < count; i++)
            {
                Charger charger = CreateNewHealer();
                ReturnToPool(charger);
            }
        }

        private Charger CreateNewHealer()
        {
            Charger charger = Instantiate(m_ChargePrefab, m_DeliverTransform);
            charger.gameObject.SetActive(false);
            charger.OnChargeDone.AddListener((ctx) => ReturnToPool(charger));
            return charger;
        }

        private Charger GetFromPool()
        {
            Charger charger;

            if (m_HealerPool.Count > 0)
                charger = m_HealerPool.Dequeue();
            else
                charger = CreateNewHealer();

            charger.transform.SetParent(null);
            charger.gameObject.SetActive(true);
            charger.Init(m_AbilityContext);
            return charger;
        }

        private void ReturnToPool(Charger charger)
        {
            if (charger == null)
                return;

            charger.gameObject.SetActive(false);
            charger.transform.SetParent(m_DeliverTransform);

            m_ActiveCharger.Remove(charger);
            m_HealerPool.Enqueue(charger);
        }
    }
}
