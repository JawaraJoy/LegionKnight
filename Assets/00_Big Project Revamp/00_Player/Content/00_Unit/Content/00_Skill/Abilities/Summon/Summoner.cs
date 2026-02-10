using System.Collections.Generic;
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
        private int m_PendingSpawnLeft;
        [SerializeField, MMReadOnly]
        private float m_FireTimer;
        [SerializeField, MMReadOnly]
        private int m_BurstLeft;
        [SerializeField, MMReadOnly]
        private float m_BurstTimer;
        [SerializeField, MMReadOnly]
        private bool m_IsSpawning;
        [SerializeField, MMReadOnly]
        private Vector3 m_SummonOrigin;
        [SerializeField, MMReadOnly]
        private List<Vector3> m_SpawnPositions;
        [SerializeField, MMReadOnly]
        private int m_SpawnIndex;

        [SerializeField, MMReadOnly]
        private int m_TotalToSpawn;

        // burst
        [SerializeField, MMReadOnly]
        private float m_BurstCooldownTimer;

        private void OnEnable()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }
        private void OnDisable()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }
        public override void Init(AbilityConfig config, SkillContext context)
        {
            base.Init(config, context);
            if (m_Config is SummonAbilityConfig summonConfig)
            {
                m_SummonConfig = summonConfig;
                m_SummonedConfig = m_SummonConfig.UnitToSpawn;
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
            if (m_SummonConfig == null) return;
            if (m_SummonConfig.UnitToSpawn == null) return;
            if (m_SummonConfig.SpawnShape == null) return;

            Unit owner = m_AbilityContext.SkillContext.ModuleContext.UnitOwner;
            if (owner == null) return;

            // total spawn diambil dari setup FireCount
            m_TotalToSpawn = Mathf.Max(1, m_SummonConfig.SpawnSetup.FireCount);

            // reset state
            m_IsSpawning = true;
            m_SpawnIndex = 0;

            m_FireTimer = 0f;

            m_BurstLeft = Mathf.Max(1, m_SummonConfig.SpawnSetup.BurstCount);
            m_BurstCooldownTimer = 0f;

            // kalau Instant, spawn semua sekarang
            if (m_SummonConfig.SpawnSetup.FireMode == FireMode.Instant)
            {
                for (int i = 0; i < m_TotalToSpawn; i++)
                {
                    SpawnOne(owner, i, m_TotalToSpawn);
                }
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
                Unit unit = CreateNew(m_SummonConfig.UnitToSpawn);
                ReturnToPoolInternal(unit);
            }
        }
        private Unit GetFromPool()
        {
            Unit unit;

            if (m_SummonedUnitPool.Count > 0)
            {
                unit = m_SummonedUnitPool.Dequeue();
            }
            else
            {
                unit = CreateNew(m_SummonedConfig);
            }

            unit.transform.SetParent(null);
            unit.gameObject.SetActive(true);
            //unit.Init(m_SummonedConfig);

            return unit;
        }
        private Unit CreateNew(UnitConfig unitConfig)
        {
            Unit unit = Instantiate(unitConfig.UnitPrefab, m_VfxSpawnPost);
            unit.gameObject.SetActive(false);
            unit.Init(unitConfig);
            return unit;
        }
        private void ReturnToPoolInternal(Unit unit)
        {
            if (unit == null)
                return;

            unit.gameObject.SetActive(false);
            unit.transform.SetParent(m_VfxSpawnPost);

            if (m_ActiveSummonedUnits.Contains(unit))
            {
                m_ActiveSummonedUnits.Remove(unit);
            }

            m_SummonedUnitPool.Enqueue(unit);
        }
        public void ReturnToPool(Unit unit)
        {
            ReturnToPoolInternal(unit);
        }

        public void Tick()
        {
            ProcessSummonTick();
        }

        private void ProcessSummonTick()
        {
            if (!m_IsSpawning) return;
            if (m_SpawnIndex >= m_TotalToSpawn)
            {
                m_IsSpawning = false;
                return;
            }

            Unit owner = m_AbilityContext.SkillContext.ModuleContext.UnitOwner;
            if (owner == null)
            {
                m_IsSpawning = false;
                return;
            }

            var setup = m_SummonConfig.SpawnSetup;

            switch (setup.FireMode)
            {
                case FireMode.Interval:
                    m_FireTimer -= Time.deltaTime;
                    if (m_FireTimer <= 0f)
                    {
                        SpawnOne(owner, m_SpawnIndex, m_TotalToSpawn);
                        m_SpawnIndex++;
                        m_FireTimer = Mathf.Max(0.01f, setup.FireInterval);
                    }
                    break;

                case FireMode.Burst:
                    // spawn burst cepat (1 per tick) lalu cooldown burst interval
                    if (m_BurstLeft > 0)
                    {
                        SpawnOne(owner, m_SpawnIndex, m_TotalToSpawn);
                        m_SpawnIndex++;
                        m_BurstLeft--;
                    }
                    else
                    {
                        m_BurstCooldownTimer -= Time.deltaTime;
                        if (m_BurstCooldownTimer <= 0f)
                        {
                            m_BurstLeft = Mathf.Max(1, setup.BurstCount);
                            m_BurstCooldownTimer = Mathf.Max(0.01f, setup.BurstInterval);
                        }
                    }
                    break;
            }
        }

        private void SpawnOne(Unit owner, int index, int totalCount)
        {
            // tentukan origin transform berdasarkan target mode
            Transform origin = ResolveOriginTransform(owner);

            // gunakan shape config untuk posisi & rotasi
            m_SummonConfig.SpawnShape.GetSpawnTransform(origin, index, totalCount, out Vector3 pos, out Quaternion rot);

            Unit unit = GetFromPool();
            unit.transform.SetPositionAndRotation(pos, rot);

            // attach controller
            var controller = unit.GetComponent<SummonControler>();
            if (controller == null)
                controller = unit.gameObject.AddComponent<SummonControler>();

            controller.Init(this); // sesuai versi kamu (Tick via UpdateBank ada di controller)

            if (!m_ActiveSummonedUnits.Contains(unit))
                m_ActiveSummonedUnits.Add(unit);
        }
        private Transform ResolveOriginTransform(Unit owner)
        {
            switch (m_SummonConfig.SummonTargetMode)
            {
                case SummonTargetMode.AroundCasterPosition:
                    return owner.transform;

                case SummonTargetMode.AroundTargetPosition:
                    // kalau kamu punya target locked dari AbilityContext / SkillActivator, taruh di sini
                    return transform;

                case SummonTargetMode.AroundPointedPosition:
                    // kalau ada aim point transform, taruh di sini
                    return transform;

                default:
                    return owner.transform;
            }
        }

    }

}
