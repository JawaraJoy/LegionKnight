using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class DrawDiscount
    {
        [SerializeField] private string m_Id;
        [SerializeField] private bool m_DiscountEnabled = true;
        [SerializeField] private bool m_FirstDrawConsumed;
        [SerializeField, Range(0f, 1f)] private float m_PriceRate = 1f;
        [SerializeField, Range(0f, 1f)] private float m_FirstDrawRate = 1f;

        public string Id => m_Id;
        public bool DiscountEnabled => m_DiscountEnabled;
        public bool FirstDrawConsumed => m_FirstDrawConsumed;
        public float PriceRate => m_PriceRate;
        public float FirstDrawRate => m_FirstDrawRate;

        public void ConsumeFirstDraw()
        {
            m_FirstDrawConsumed = true;
        }
    }
}
