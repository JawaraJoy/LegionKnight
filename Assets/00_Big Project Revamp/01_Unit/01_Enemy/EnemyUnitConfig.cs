using LegionKnight;
using UnityEngine;

namespace Rush
{
    public abstract partial class EnemyUnitConfig : UnitConfig
    {
        [SerializeField]
        private int m_ExpReward; // used for roguelike level up system
        [SerializeField]
        private int m_ScoreReward; // used for score system
        public int ExpReward => m_ExpReward;
        public int ScoreReward => m_ScoreReward;
    }
}
