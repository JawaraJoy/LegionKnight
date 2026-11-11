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
        private float m_ValueRate = 1;
        [SerializeField]
        private int m_Value = 0;
        public int Value => m_Value;
        public float ValueRate => m_ValueRate;
    }
}
