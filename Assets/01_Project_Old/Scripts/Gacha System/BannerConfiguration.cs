using System.Collections.Generic;
using UnityEngine;
using Rush;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Banner_", menuName = "Legion Knight/Banner")]
    public class BannerConfiguration : Configuration
    {
        [Header("Visual")]
        [SerializeField] private Sprite m_VisualBanner;
        [SerializeField] private Sprite m_SmallVisualBanner;
        [SerializeField, TextArea] private string m_PromoText;

        [Header("Draw Rules")]
        [SerializeField] private int m_MultiDraw = 10;
        [SerializeField] private int m_SmallPity = 10;
        [SerializeField] private int m_GuaranteedDraw = 120;

        [Header("Soft Pity")]
        [SerializeField] private bool m_EnableSoftPity;
        [SerializeField] private int m_SoftPityStart = 80;
        [SerializeField] private float m_SoftPityMultiplier = 1.5f;

        [Header("Seasonal")]
        [SerializeField] private bool m_IsSeasonal;
        [SerializeField] private long m_SeasonDurationSeconds;

        [Header("Currency")]
        [SerializeField] private GachaCurrencyCost m_MainCurrency;
        [SerializeField] private GachaCurrencyCost m_AlternativeCurrency;

        [Header("Rewards")]
        [SerializeField] private GachaRewardConfig m_FirstDrawReward;
        [SerializeField] private List<GachaRewardConfig> m_MainRewards = new();
        [SerializeField] private List<GachaRewardConfig> m_SmallPityRewards = new();
        [SerializeField] private List<GachaRewardConfig> m_NormalRewards = new();
        public string PromoText => m_PromoText;

        public int MultiDraw => m_MultiDraw;
        public int SmallPity => m_SmallPity;
        public int GuaranteedDraw => m_GuaranteedDraw;

        public bool EnableSoftPity => m_EnableSoftPity;
        public int SoftPityStart => m_SoftPityStart;
        public float SoftPityMultiplier => m_SoftPityMultiplier;
        public Sprite VisualBanner => m_VisualBanner;
        public Sprite SmallVisualBanner => m_SmallVisualBanner;

        public bool IsSeasonal => m_IsSeasonal;
        public long SeasonDurationSeconds => m_SeasonDurationSeconds;

        public GachaCurrencyCost MainCurrency => m_MainCurrency;
        public GachaCurrencyCost AlternativeCurrency => m_AlternativeCurrency;

        public GachaRewardConfig FirstDrawReward => m_FirstDrawReward;
        public IReadOnlyList<GachaRewardConfig> MainRewards => m_MainRewards;
        public IReadOnlyList<GachaRewardConfig> SmallPityRewards => m_SmallPityRewards;
        public IReadOnlyList<GachaRewardConfig> NormalRewards => m_NormalRewards;
    }
}
