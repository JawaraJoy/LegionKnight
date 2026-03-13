using System.Collections.Generic;
using LegionKnight;
using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class Summoner : AbilityDeliver, IUpdater
    {
        [SerializeField, MMReadOnly]
        private SummonAbilityConfig m_SummonConfig;
        public SummonAbilityConfig SummonConfig => m_SummonConfig;

        public bool IsActive => gameObject.activeSelf;

        [SerializeField, MMReadOnly]
        private List<Unit> m_ActiveSummonedUnits = new();

        [SerializeField, MMReadOnly]
        private Queue<Unit> m_SummonedUnitPool = new();

        private UnitConfig m_SummonedConfig;

        [SerializeField, MMReadOnly]
        private float m_FireTimer;

        [SerializeField, MMReadOnly]
        private float m_BurstTimer;

        [SerializeField, MMReadOnly]
        private int m_BurstRemaining;

        [SerializeField, MMReadOnly]
        private bool m_IsSpawning;

        [SerializeField, MMReadOnly]
        private bool m_LoopForward = true;

        [SerializeField, MMReadOnly]
        private int m_CurrentIndex;

        [SerializeField, MMReadOnly]
        private int m_TotalJobs;

        [SerializeField, MMReadOnly]
        private int m_ExecutedJobs;

        [SerializeField, MMReadOnly]
        private Queue<SummonJob> m_SummonJobs = new();

        [SerializeField, MMReadOnly]
        private List<SummonJob> m_LoopJobs = new();
        public List<Unit> ActiveSummonedUnits => m_ActiveSummonedUnits;
        private void OnEnable()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }

        private void OnDisable()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        public override void Init(AbilityConfig config, ISkillContext context)
        {
            base.Init(config, context);

            if (config is SummonAbilityConfig summonConfig)
            {
                m_SummonConfig = summonConfig;
                m_SummonedConfig = summonConfig.UnitToSpawn;
                PreWarm();
            }
        }

        public override void Activate()
        {
            Summon();
            base.Activate();
        }

        private void Summon()
        {
            if (m_SummonConfig == null ||
                m_SummonConfig.SpawnShape == null ||
                m_SummonConfig.UnitToSpawn == null ||
                m_SummonConfig.SpawnSetup == null)
                return;

            var setup = m_SummonConfig.SpawnSetup;

            m_SummonJobs.Clear();
            m_LoopJobs.Clear();

            m_IsSpawning = false;
            m_FireTimer = 0f;
            m_BurstTimer = 0f;
            m_CurrentIndex = 0;
            m_LoopForward = true;
            m_ExecutedJobs = 0;
            m_TotalJobs = 0;

            List<ITargetable> targets = GetTargetsInternal();
            if (targets == null || targets.Count == 0)
                return;

            int perTargetCount = Mathf.Max(1, setup.BurstCount);

            foreach (var target in targets)
            {
                Transform origin = target.TargetTransform;

                for (int i = 0; i < perTargetCount; i++)
                {
                    SummonJob job = new SummonJob
                    {
                        Origin = origin,
                        SlotIndex = i,
                        SlotCount = perTargetCount
                    };

                    m_SummonJobs.Enqueue(job);
                    m_LoopJobs.Add(job);
                }
            }

            m_TotalJobs = m_SummonJobs.Count;
            if (m_TotalJobs == 0)
                return;

            m_IsSpawning = true;
            m_BurstRemaining = Mathf.Max(1, setup.BurstCount);

            if (setup.FireMode == FireMode.Instant)
            {
                while (m_SummonJobs.Count > 0)
                {
                    SpawnJob(m_SummonJobs.Dequeue());
                }

                m_IsSpawning = false;
            }
        }

        public void Tick()
        {
            if (!m_IsSpawning || m_SummonConfig == null)
                return;

            var setup = m_SummonConfig.SpawnSetup;

            switch (setup.FireMode)
            {
                case FireMode.Interval:
                    HandleInterval(setup);
                    break;

                case FireMode.Burst:
                    HandleBurst(setup);
                    break;

                case FireMode.Loop:
                    HandleLoop(setup);
                    break;

                case FireMode.PingPong:
                    HandlePingPong(setup);
                    break;

                case FireMode.Random:
                    HandleRandom(setup);
                    break;
            }
        }

        private void HandleInterval(SpawnSetupField setup)
        {
            if (m_SummonJobs.Count == 0)
            {
                m_IsSpawning = false;
                return;
            }

            m_FireTimer -= Time.deltaTime;

            if (m_FireTimer <= 0f)
            {
                SpawnJob(m_SummonJobs.Dequeue());
                m_FireTimer = Mathf.Max(0.01f, setup.FireInterval);
            }
        }

        private void HandleBurst(SpawnSetupField setup)
        {
            if (m_SummonJobs.Count == 0)
            {
                m_IsSpawning = false;
                return;
            }

            if (m_BurstRemaining > 0)
            {
                SpawnJob(m_SummonJobs.Dequeue());
                m_BurstRemaining--;
            }
            else
            {
                m_BurstTimer -= Time.deltaTime;

                if (m_BurstTimer <= 0f)
                {
                    m_BurstRemaining = Mathf.Max(1, setup.BurstCount);
                    m_BurstTimer = Mathf.Max(0.01f, setup.BurstInterval);
                }
            }
        }

        private void HandleLoop(SpawnSetupField setup)
        {
            if (m_LoopJobs.Count == 0)
                return;

            m_FireTimer -= Time.deltaTime;

            if (m_FireTimer <= 0f)
            {
                SpawnJob(m_LoopJobs[m_CurrentIndex]);

                m_CurrentIndex++;
                if (m_CurrentIndex >= m_LoopJobs.Count)
                    m_CurrentIndex = 0;

                m_FireTimer = Mathf.Max(0.01f, setup.FireInterval);
            }
        }

        private void HandlePingPong(SpawnSetupField setup)
        {
            if (m_LoopJobs.Count == 0)
                return;

            m_FireTimer -= Time.deltaTime;

            if (m_FireTimer <= 0f)
            {
                SpawnJob(m_LoopJobs[m_CurrentIndex]);

                if (m_LoopForward)
                {
                    m_CurrentIndex++;
                    if (m_CurrentIndex >= m_LoopJobs.Count - 1)
                        m_LoopForward = false;
                }
                else
                {
                    m_CurrentIndex--;
                    if (m_CurrentIndex <= 0)
                        m_LoopForward = true;
                }

                m_FireTimer = Mathf.Max(0.01f, setup.FireInterval);
            }
        }

        private void HandleRandom(SpawnSetupField setup)
        {
            if (m_LoopJobs.Count == 0)
                return;

            m_FireTimer -= Time.deltaTime;

            if (m_FireTimer <= 0f)
            {
                int randomIndex = Random.Range(0, m_LoopJobs.Count);
                SpawnJob(m_LoopJobs[randomIndex]);

                m_FireTimer = Mathf.Max(0.01f, setup.FireInterval);
            }
        }

        private void SpawnJob(SummonJob job)
        {
            if (job.Origin == null)
                return;

            m_SummonConfig.SpawnShape.GetSpawnTransform(
                job.Origin,
                job.SlotIndex,
                job.SlotCount,
                out Vector3 pos,
                out Quaternion rot);

            Unit unit = GetFromPool();
            unit.transform.SetPositionAndRotation(pos, rot);
            unit.Init(m_SummonedConfig);
            if (unit is SummonerUnit summonerUnit)
            {
                summonerUnit.SetSummoner(this);

                if (!m_ActiveSummonedUnits.Contains(unit))
                    m_ActiveSummonedUnits.Add(unit);

                m_ExecutedJobs++;

                if (m_ExecutedJobs >= m_TotalJobs)
                    m_IsSpawning = false;
            }
        }

        private void PreWarm()
        {
            if (m_SummonConfig == null || m_SummonConfig.UnitToSpawn == null)
                return;

            int count = m_SummonConfig.SpawnSetup.PreWarmCount;

            for (int i = 0; i < count; i++)
            {
                Unit unit = Instantiate(m_SummonConfig.UnitToSpawn.UnitPrefab, m_DeliverTransform);
                unit.transform.LeanScale(unit.Config.UnitScale, 0.3f).setEaseLinear().setFrom(Vector3.one);
                unit.gameObject.SetActive(false);
                unit.Init(m_SummonConfig.UnitToSpawn);
                m_SummonedUnitPool.Enqueue(unit);
            }
        }

        private Unit GetFromPool()
        {
            Unit unit;

            if (m_SummonedUnitPool.Count > 0)
                unit = m_SummonedUnitPool.Dequeue();
            else
                unit = Instantiate(m_SummonedConfig.UnitPrefab, m_DeliverTransform);
                unit.transform.LeanScale(unit.Config.UnitScale, 0.3f).setEaseOutBack();

            unit.transform.SetParent(null);
            unit.gameObject.SetActive(true);

            return unit;
        }

        public void ReturnToPool(Unit unit)
        {
            if (unit == null)
                return;

            unit.gameObject.SetActive(false);
            unit.transform.SetParent(m_DeliverTransform);

            m_ActiveSummonedUnits.Remove(unit);
            m_SummonedUnitPool.Enqueue(unit);
        }
    }
}
