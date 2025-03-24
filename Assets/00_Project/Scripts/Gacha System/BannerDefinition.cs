using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Banner", menuName = "Legion Knight/Banner")]
    public partial class BannerDefinition : ScriptableObject
    {
        [SerializeField]
        private int m_GuaranteedDraw = 120;
        [SerializeField]
        private int m_MultiDraw = 10;
        [SerializeField]
        private List<GachaReward> m_MainRewards;
        [SerializeField]
        private GachaCurrencyCost m_MainCurrencyToDraw;
        [SerializeField]
        private GachaCurrencyCost m_AlternatifCurrencyToDraw;
        
        [SerializeField]
        private List<GachaReward> m_GachaRewards = new();
        public int MultiDraw => m_MultiDraw;
        public int GuaranteedDraw => m_GuaranteedDraw;
        public List<GachaReward> MainRewards => m_MainRewards;
        public GachaCurrencyCost MainCurrencyToDraw => m_MainCurrencyToDraw;
        public GachaCurrencyCost AlternatifCurrencyToDraw => m_AlternatifCurrencyToDraw;
        public List<GachaReward> GachaRewards => m_GachaRewards;
    }
    [System.Serializable]
    public partial class GachaReward
    {
        [SerializeField]
        private string m_RewardName;
        [SerializeField]
        private int m_Amount;
        [SerializeField]
        private float m_DropRate;
        public string RewardName => m_RewardName;
        public float DropRate => m_DropRate;
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
    }
    [System.Serializable]
    public partial class DrawDiscount
    {
        [SerializeField]
        private bool m_Used;
        [SerializeField, Range(0f, 1f)]
        private float m_Discount;
        public bool Used => m_Used;
        public float Discount => m_Discount;
        public void SetUsed(bool set)
        {
            m_Used = set;
        }
    }
}
