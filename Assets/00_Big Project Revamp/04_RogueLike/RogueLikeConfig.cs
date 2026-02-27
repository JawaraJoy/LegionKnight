using UnityEngine;
using LegionKnight;

namespace Rush
{
    [CreateAssetMenu(fileName = "RogueLikeConfig", menuName = "Rush/RogueLike/RogueLikeConfig", order = 0)]
    public class RogueLikeConfig : Configuration
    {
        [SerializeField]
        private RogueLevelFormula m_LevelFormula;
        public RogueLevelFormula LevelFormula => m_LevelFormula;
    }
}
