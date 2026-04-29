using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Banner_", menuName = "Rush/Gacha/Banner")]
    public class GachaBannerConfig : Configuration
    {
        [Header("Visuals")]
        [SerializeField] private Sprite m_BannerButtonSprite;
        [SerializeField] private Sprite m_BannerSplashSprite;

        [Header("Collectables")]
        [SerializeField] private GachaCollectableConfig[] m_Collectables;

        [Header("Pity — First Draw")]
        [SerializeField] private bool m_HasFirstDrawGuarantee = true;
        [SerializeField] private GachaCollectableConfig[] m_FirstDrawGuarantees;

        [Header("Pity — Small")]
        [SerializeField] private int m_SmallPityCount = 10;
        [SerializeField] private GachaCollectableConfig[] m_SmallPityGuarantees;

        [Header("Pity — Final")]
        [SerializeField] private int m_FinalPityCount = 50;
        [SerializeField] private GachaCollectableConfig[] m_FinalPityGuarantees;

        [Header("Draw Cost")]
        [SerializeField] private ItemConfig m_DrawCostCurrency;
        [SerializeField] private int m_SingleDrawCost = 160;
        [SerializeField] private ItemConfig m_AltCostCurrency;
        [SerializeField] private int m_AltSingleDrawCost = 1;

        [Header("Multi Draw")]
        [SerializeField] private int m_MultiDrawCount = 10;

        [Header("Discounts")]
        [SerializeField] private GachaDiscountConfig m_DiscountConfig;

        public Sprite BannerButtonSprite => m_BannerButtonSprite;
        public Sprite BannerSplashSprite => m_BannerSplashSprite;
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