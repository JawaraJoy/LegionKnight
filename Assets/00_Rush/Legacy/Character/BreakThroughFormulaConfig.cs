using Rush;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Hero Evolution", menuName = "Rush/Hero/Evolution")]
    public class BreakThroughFormulaConfig : ScriptableObject
    {
        [SerializeField] private ItemConfig m_ShardConfig;
        [SerializeField] private StatField[] m_StageBonuses;
        public ItemConfig ShardConfig => m_ShardConfig;

        public StatField GetStatBonus(int stage)
        {
            if (m_StageBonuses == null || m_StageBonuses.Length == 0) return StatField.Zero;
            int last = m_StageBonuses.Length - 1;
            stage = Mathf.Clamp(stage, 0, HeroEvolution.MaxStage);
            if (stage <= last) return m_StageBonuses[stage];
            return m_StageBonuses[last] * (stage - last + 1);
        }
    }
}
