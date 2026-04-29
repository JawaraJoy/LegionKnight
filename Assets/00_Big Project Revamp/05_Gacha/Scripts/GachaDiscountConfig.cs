using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "GachaDiscount_", menuName = "Rush/Gacha/Discount Config")]
    public class GachaDiscountConfig : ScriptableObject
    {
        [SerializeField, Range(0f, 1f)] private float m_SingleDrawDailyDiscount = 0f;
        [SerializeField, Range(0f, 1f)] private float m_MultiDrawDailyDiscount = 0f;
        [SerializeField, Range(0f, 1f)] private float m_MultiDrawDiscount = 0f;
        [SerializeField, Range(0f, 1f)] private float m_GeneralDiscount = 0f;

        [Tooltip("Discount tidak berlaku jika base cost sama atau kurang dari nilai ini")]
        [SerializeField] private int m_MinimumPriceForDiscount = 0;

        public float SingleDrawDailyDiscount => m_SingleDrawDailyDiscount;      
        public float MultiDrawDailyDiscount => m_MultiDrawDailyDiscount;
        public float MultiDrawDiscount => m_MultiDrawDiscount;
        public float GeneralDiscount => m_GeneralDiscount;
        public int MinimumPriceForDiscount => m_MinimumPriceForDiscount;
    }
}