using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class RarityConfig : Configuration
    {
        [SerializeField]
        private Color m_Color = Color.white;
        public Color Color => m_Color;
    }
}
