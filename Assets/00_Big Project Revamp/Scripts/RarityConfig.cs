using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Rarity_", menuName = "Rush/Rarity")]
    public partial class RarityConfig : Configuration
    {
        [SerializeField]
        private Color m_Color = Color.white;
        public Color Color => m_Color;
        [SerializeField]
        private int m_SacrificeCost = 0;
        public int SacrificeCost => m_SacrificeCost;
    }
}
