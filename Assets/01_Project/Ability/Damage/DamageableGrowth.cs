using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class DamageableGrowth : MonoBehaviour
    {
        private int m_Level = 1; // Current level of the damageable growth
        [SerializeField]
        private Stat m_BaseStat;
        [SerializeField]
        private Stat m_GrowthStat;
        [SerializeField]
        private UnityEvent<int> m_OnLevelUpdate = new();
        [SerializeField]
        private UnityEvent<Stat> m_OnStatUpdate = new();
        public void SetLevel(int level)
        {
            m_Level = level;
            // Invoke the event to notify listeners about the level update
            m_OnLevelUpdate.Invoke(level);
            UpdateStatsInternal(level);
        }

        private void UpdateStatsInternal(int level)
        {
            // Update the stats based on the current level
            Stat final = Stat.GetStatByLevel(m_BaseStat, m_GrowthStat, level);
            // Invoke the event to notify listeners about the stat update
            m_OnStatUpdate.Invoke(final);

            if (TryGetComponent<Damageable>(out var damageable))
            {
                // Update the damageable's stats
                damageable.SetDamage(final.Attack);
                damageable.SetDefend(final.Defense);
                damageable.SetHealth(final.Health);
            }
        }

        public void UpdateStat(int level)
        {
            if (level != m_Level)
            {
                m_Level = level;
                UpdateStatsInternal(level);
            }
        }
    }
}
