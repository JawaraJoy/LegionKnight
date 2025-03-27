using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Banner", menuName = "Legion Knight/Banner")]
    public partial class BannerDefinition : ScriptableObject
    {
        [SerializeField]
        private Sprite m_VisualBanner;
        [SerializeField]
        private Sprite m_SmallVisualBanner;
        [SerializeField, TextArea]
        private string m_PromoText;
        [SerializeField]
        private int m_GuaranteedDraw = 120;
        [SerializeField]
        private int m_MultiDraw = 10;
        
        [SerializeField]
        private GachaCurrencyCost m_MainCurrencyToDraw;
        [SerializeField]
        private GachaCurrencyCost m_AlternatifCurrencyToDraw;
        [SerializeField]
        private List<GachaReward> m_MainRewards = new();
        [SerializeField]
        private List<GachaReward> m_GachaRewards = new();
        public int MultiDraw => m_MultiDraw;
        public int GuaranteedDraw => m_GuaranteedDraw;
        public List<GachaReward> MainRewards => m_MainRewards;
        public GachaCurrencyCost MainCurrencyToDraw => m_MainCurrencyToDraw;
        public GachaCurrencyCost AlternatifCurrencyToDraw => m_AlternatifCurrencyToDraw;
        public string PromoText => m_PromoText;
        public List<GachaReward> GachaRewards => m_GachaRewards;
        public Sprite VisualBanner => m_VisualBanner;
        public Sprite SmallVisualBanner => m_SmallVisualBanner;
    }
    [System.Serializable]
    public partial class GachaReward
    {
        [SerializeField]
        private ScriptableObject m_Definition;
        [SerializeField]
        private int m_Amount;
        [SerializeField, Range(0f, 1f)]
        private float m_DropRate;
        public ScriptableObject Definition => m_Definition;
        public float DropRate => m_DropRate;
        public int Amount => m_Amount;

        public GachaReward(ScriptableObject definition, int amount, float dropRate)
        {
            m_Definition = definition;
            m_Amount = amount;
            m_DropRate = dropRate;
        }

    }
    [System.Serializable]
    public partial class GachaCurrencyCost
    {
        [SerializeField]
        private CurrencyDefinition m_Definition;
        [SerializeField]
        private int m_Amount;
        public CurrencyDefinition Definition => m_Definition;
        public int Amount => m_Amount;

        public GachaCurrencyCost(CurrencyDefinition definition, int amount)
        {
            m_Definition = definition;
            m_Amount = amount;
        }
    }
    [System.Serializable]
    public partial class DrawDiscount
    {
        [SerializeField]
        private bool m_Used;
        [SerializeField, Range(0f, 1f)]
        private float m_PriceRate   ;
        public bool Used => m_Used;
        public float PriceRate => m_PriceRate;
        public void SetUsed(bool set)
        {
            m_Used = set;
        }
    }
}
