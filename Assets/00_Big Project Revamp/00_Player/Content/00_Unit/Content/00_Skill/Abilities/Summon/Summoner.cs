using System.Collections.Generic;
using LegionKnight;
using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class Summoner : AbilityDeliver
    {
        [SerializeField, MMReadOnly]
        private SummonAbilityConfig m_SummonConfig;
        public SummonAbilityConfig SummonConfig => m_SummonConfig;

        [SerializeField, MMReadOnly]
        private List<Unit> m_ActiveSummonedUnits = new();

        [SerializeField, MMReadOnly]
        private Queue<Unit> m_SummonedUnitPool = new();

        private UnitConfig m_SummonedConfig;
        private void Awake()
        {
            PreWarm();
        }
        public override void Init(AbilityConfig config, SkillContext context)
        {
            base.Init(config, context);
            if (m_Config is SummonAbilityConfig summonConfig)
            {
                m_SummonConfig = summonConfig;
                m_SummonedConfig = m_SummonConfig.UnitToSpawn;
            }
        }
        public override void Activate()
        {
            Summon();
            base.Activate();
        }

        private void Summon()
        {

        }
        private void PreWarm()
        {
            if (m_SummonConfig == null || m_SummonConfig.UnitToSpawn == null)
                return;

            int count = m_SummonConfig.SpawnSetup.PreWarmCount;

            for (int i = 0; i < count; i++)
            {
                Unit unit = CreateNew(m_SummonConfig.UnitToSpawn);
                ReturnToPool(unit);
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
            unit.Init(m_SummonedConfig);

            return unit;
        }
        private Unit CreateNew(UnitConfig unitConfig)
        {
            Unit unit = Instantiate(unitConfig.UnitPrefab, m_VfxSpawnPost);
            unit.gameObject.SetActive(false);
            unit.Init(unitConfig);
            return unit;
        }
        private void ReturnToPool(Unit unit)
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
    }
}
