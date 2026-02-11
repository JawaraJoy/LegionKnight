using Unity.Cinemachine;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Hero", menuName = "Rush/Unit/Hero", order = 0)]
    public partial class HeroConfig : UnitConfig
    {
        [SerializeField, Min(0)]
        private int m_StartingStars;
        [SerializeField, Min(0)]
        private int m_MaxStars;
        [SerializeField]
        private bool m_UseAsDefault = false;
        [SerializeField]
        private bool m_OwnedAtFirst = false;

        public int StartingStars => m_StartingStars;
        public int MaxStars => m_MaxStars;
        public bool UseAsDefault => m_UseAsDefault;
        public bool OwnedAtFirst => m_OwnedAtFirst;
    }
}
