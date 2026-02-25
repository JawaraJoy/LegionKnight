using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Rarity", menuName = "Rush/Rarity")]
    public partial class RarityConfig : Configuration
    {
        [SerializeField]
        private Color m_Color = Color.white;
        public Color Color => m_Color;
    }
}
