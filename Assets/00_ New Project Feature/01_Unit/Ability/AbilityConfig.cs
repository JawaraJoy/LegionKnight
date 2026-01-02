using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Rush/Character/Ability", order = 1)]
    public partial class AbilityConfig : Configuration
    {
        [SerializeField]
        private AbilityActivator m_ActivatorPrefab;
        [SerializeField]
        private AbilityStatsField m_Stats;
        public AbilityActivator ActivatorPrefab => m_ActivatorPrefab;
        public AbilityStatsField Stats => m_Stats;
    }
    [System.Serializable]
    public class AbilityStatsField : StatsProgressField
    {
        [SerializeField]
        private bool m_ScaleWithCharacterStats = true;
        public bool ScaleWithCharacterStats => m_ScaleWithCharacterStats;
    }
    public partial class AbilityContext
    {
        public virtual StatField GetFinalStat()
        {
            bool scaleWithCharacterStats = Config.Stats.ScaleWithCharacterStats;
            StatsProgressField characterStats = m_Owner.Config.MainStats;
            StatField charFinalStat = characterStats.GetFinalStat();
            StatField abilityBaseStat = Config.Stats.BaseStat;
            StatField abilityFlatStat = Config.Stats.FlatScaleByLevel;
            StatField abilityPercentStat = Config.Stats.PercentScaleByLevel;

            StatField finalStat = m_Config.Stats.GetFinalStat();
            if (scaleWithCharacterStats)
            {
                StatField scaleBaseStat = characterStats.BaseStat + abilityBaseStat;
                scaleBaseStat += StatField.GetFinalFlatStats(abilityFlatStat, m_CurrentLevel);
                scaleBaseStat += StatField.GetFinalRateStats(abilityPercentStat, m_CurrentLevel) * charFinalStat;
                finalStat = scaleBaseStat;
            }
            return finalStat;
        }
    }
}
