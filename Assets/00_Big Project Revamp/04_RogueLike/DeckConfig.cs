using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Deck_", menuName = "Rush/RogueLike/Deck", order = 0)]
    public class DeckConfig : Configuration
    {
        [SerializeField]
        private CardConfig[] m_CardConfigs;
        public CardConfig[] CardConfigs => m_CardConfigs;
    }
}
