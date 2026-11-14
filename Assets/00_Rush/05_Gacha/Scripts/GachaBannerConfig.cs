using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Banner_", menuName = "Rush/Gacha/Banner")]
    public class GachaBannerConfig : Configuration
    {

        [SerializeField] private Sprite m_BannerButtonSprite;
        [SerializeField] private Sprite m_BannerSplashSprite;

        [SerializeField] private GachaCollectableConfig[] m_Collectables;

        [SerializeField] private bool m_HasFirstDrawGuarantee = true;
        [SerializeField] private GachaCollectableConfig[] m_FirstDrawGuarantees;

        [SerializeField] private int m_SmallPityCount = 10;
        [SerializeField] private GachaCollectableConfig[] m_SmallPityGuarantees;

        [SerializeField] private int m_FinalPityCount = 50;
        [SerializeField] private GachaCollectableConfig[] m_FinalPityGuarantees;

        [SerializeField] private ItemConfig m_DrawCostCurrency;
        [SerializeField] private int m_SingleDrawCost = 160;
        [SerializeField] private ItemConfig m_AltCostCurrency;
        [SerializeField] private int m_AltSingleDrawCost = 1;

        [SerializeField] private int m_MultiDrawCount = 10;

        [SerializeField] private GachaDiscountConfig m_DiscountConfig;

        [System.Serializable]
        private struct PanelPresentationField
        {
            [SerializeField] private string m_HeroName;
            [SerializeField] private string m_Epithet;
            [SerializeField] private Sprite m_Artwork;
            [SerializeField, Range(0f, 0.9f)] private float m_BottomCrop;
            [Tooltip("Normalized bounds within the banner stage. Leave empty to use the panel layout.")]
            [SerializeField] private Rect m_ArtworkBounds;

            public string HeroName => m_HeroName;
            public string Epithet => m_Epithet;
            public Sprite Artwork => m_Artwork;
            public float BottomCrop => m_BottomCrop;
            public Rect ArtworkBounds => m_ArtworkBounds;
        }

        [SerializeField] private PanelPresentationField m_PanelPresentation;
        public bool HasPanelHeroName => !string.IsNullOrWhiteSpace(m_PanelPresentation.HeroName);
        public string PanelHeroName => HasPanelHeroName ? m_PanelPresentation.HeroName : BaseInfo.Name;
        public string PanelEpithet => m_PanelPresentation.Epithet;
        public Sprite PanelArtwork => m_PanelPresentation.Artwork != null ? m_PanelPresentation.Artwork : m_BannerSplashSprite;
        public float PanelBottomCrop => m_PanelPresentation.BottomCrop;
        public Rect PanelArtworkBounds => m_PanelPresentation.ArtworkBounds;

        public Sprite BannerButtonSprite => m_BannerButtonSprite;
        public Sprite BannerSplashSprite => m_BannerSplashSprite;
        [System.Serializable]
        private sealed class CardWeightField
        {
            [SerializeField] internal RarityConfig rarity;
            [SerializeField] internal float multiplier = 1f;
        }
        [SerializeField] private CardWeightField[] m_CardPoolWeights;

        internal float NormalWeight(GachaCollectableConfig entry)
        {
            if (entry.Collect is CardConfig && m_CardPoolWeights != null)
                foreach (var group in m_CardPoolWeights)
                    if (group.rarity == entry.Collect.CollectibleField.RarityConfig) return entry.Chance * group.multiplier;
            return entry.Chance;
        }
        public GachaCollectableConfig[] Collectables => m_Collectables;
        public bool HasFirstDrawGuarantee => m_HasFirstDrawGuarantee;
        public GachaCollectableConfig[] FirstDrawGuarantees => m_FirstDrawGuarantees;
        public int SmallPityCount => m_SmallPityCount;
        public GachaCollectableConfig[] SmallPityGuarantees => m_SmallPityGuarantees;
        public int FinalPityCount => m_FinalPityCount;
        public GachaCollectableConfig[] FinalPityGuarantees => m_FinalPityGuarantees;
        public ItemConfig DrawCostCurrency => m_DrawCostCurrency;
        public int SingleDrawCost => m_SingleDrawCost;
        public ItemConfig AltCostCurrency => m_AltCostCurrency;
        public int AltSingleDrawCost => m_AltSingleDrawCost;
        public int MultiDrawCount => m_MultiDrawCount;
        public GachaDiscountConfig DiscountConfig => m_DiscountConfig;
    }
}
