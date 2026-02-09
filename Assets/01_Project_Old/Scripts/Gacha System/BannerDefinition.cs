using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Banner", menuName = "Legion Knight/Banner")]
    public class BannerDefinition : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string m_Id;
        [SerializeField] private string m_Label;
        [SerializeField, TextArea] private string m_Description;

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
        [SerializeField] private GachaReward m_FirstDrawReward;
        [SerializeField] private List<GachaReward> m_MainRewards = new();
        [SerializeField] private List<GachaReward> m_SmallPityRewards = new();
        [SerializeField] private List<GachaReward> m_NormalRewards = new();

        public string Id => m_Id;
        public string Label => m_Label;
        public string Description => m_Description;
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

        public GachaReward FirstDrawReward => m_FirstDrawReward;
        public IReadOnlyList<GachaReward> MainRewards => m_MainRewards;
        public IReadOnlyList<GachaReward> SmallPityRewards => m_SmallPityRewards;
        public IReadOnlyList<GachaReward> NormalRewards => m_NormalRewards;
    }
}
