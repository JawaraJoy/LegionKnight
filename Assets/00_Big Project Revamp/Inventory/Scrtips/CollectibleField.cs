using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class CollectibleField : IHasIcon, IHasSplashImage, IHasUnique, IHasRarityConfig
    {
        [SerializeField]
        private RarityConfig m_RarityConfig;
        [SerializeField]
        private bool m_IsUnique;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private Sprite m_SplashImage;
        public Sprite Icon => m_Icon;
        public Sprite SplashImage => m_SplashImage;
        public bool IsUnique => m_IsUnique;

        public RarityConfig RarityConfig => m_RarityConfig;
        public CollectibleField(RarityConfig rarityConfig, bool isUnique, Sprite icon, Sprite splashImage)
        {
            m_RarityConfig = rarityConfig;
            m_IsUnique = isUnique;
            m_Icon = icon;
            m_SplashImage = splashImage;
        }
    }
}
