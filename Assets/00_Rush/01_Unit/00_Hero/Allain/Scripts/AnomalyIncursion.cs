using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class AnomalyIncursion : AbilityDeliver
    {
        private readonly List<Unit> m_Entities = new List<Unit>();
        protected override void ActivateInternal()
        {
            var config = (AnomalyIncursionConfig)m_AbilityConfig;
            var handler = RushGameManager.Instance.StageManager.EnemyWaveHandler;
            m_Entities.RemoveAll(unit => unit == null);
            for (int i = 0; i < config.Entity.Count; i++)
            {
                if (!handler.TryGetRandomSpawnPose(out Vector3 position, out Quaternion rotation))
                {
                    Debug.LogWarning("Anomaly Incursion: enemy spawner is not ready.", this);
                    break;
                }
                Unit unit = Instantiate(config.Entity.Unit.UnitPrefab, position, rotation);
                if (unit.TryGetComponent(out AnomalyEntity entity))
                    entity.Configure(config.Entity, m_AbilityContext.SkillContext);
                m_Entities.Add(unit);
                unit.Init(config.Entity.Unit);
            }
            base.ActivateInternal();
        }

        private void OnDisable()
        {
            foreach (var entity in m_Entities)
                if (entity != null) Destroy(entity.gameObject);
            m_Entities.Clear();
        }
    }
}
