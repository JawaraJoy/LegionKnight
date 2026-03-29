using LegionKnight;
using System.Linq;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "RogueLikeConfig", menuName = "Rush/RogueLike/RogueLikeConfig", order = 0)]
    public class RogueLikeConfig : Configuration
    {
        [SerializeField]
        private RogueLevelFormula m_ForPlayerLevelFormula;
        [SerializeField]
        private DeckConfig m_BaseDeck;
        public RogueLevelFormula ForPlayerLevelFormula => m_ForPlayerLevelFormula;
        public DeckConfig BaseDeck => m_BaseDeck;
    }
}
