using LegionKnight;
using System.Linq;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "RogueLikeConfig", menuName = "Rush/RogueLike/RogueLikeConfig", order = 0)]
    public class RogueLikeConfig : Configuration
    {
        [SerializeField]
        private RogueLevelFormula m_LevelFormula;
        [SerializeField]
        private CardConfig[] m_CardConfigs;
        public RogueLevelFormula LevelFormula => m_LevelFormula;
        public CardConfig[] CardConfigs => m_CardConfigs;

        public CardConfig[] GetDifferenceCardRandom(int drawAmount)
        {
            if (m_CardConfigs == null || m_CardConfigs.Length == 0)
                return new CardConfig[0];

            int drawCount = Mathf.Min(drawAmount, m_CardConfigs.Length);

            return m_CardConfigs.OrderBy(x => Random.value).Take(drawCount).ToArray();
        }
    }
}
