using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class EffectField
    {
        [SerializeField]
        private int m_InitialAmount;
        [SerializeField]
        private float m_MultiplierAmount;
        public int InitialAmount => m_InitialAmount;
        public float MultiplierAmount => m_MultiplierAmount;
        public EffectField Zero
        {
            get
            {
                return new EffectField
                {
                    m_InitialAmount = 0,
                    m_MultiplierAmount = 0f,
                };
            }
        }

        public static EffectField operator+(EffectField a, EffectField b)
        {
            return new EffectField
            {
                m_InitialAmount = a.m_InitialAmount + b.m_InitialAmount,
                m_MultiplierAmount = a.m_MultiplierAmount + b.m_MultiplierAmount,
            };
        }
        public static EffectField operator -(EffectField a, EffectField b)
        {
            return new EffectField
            {
                m_InitialAmount = a.m_InitialAmount - b.m_InitialAmount,
                m_MultiplierAmount = a.m_MultiplierAmount - b.m_MultiplierAmount,
            };
        }
        public static EffectField operator*(EffectField a, EffectField b)
        {
            return new EffectField
            {
                m_InitialAmount = (int)(a.m_InitialAmount * b.m_InitialAmount),
                m_MultiplierAmount = a.m_MultiplierAmount * b.m_MultiplierAmount,
            };
        }
        public static EffectField operator*(EffectField a, int scalar)
        {
            return new EffectField
            {
                m_InitialAmount = a.m_InitialAmount * scalar,
                m_MultiplierAmount = a.m_MultiplierAmount * scalar,
            };
        }

        public static EffectField GetFinalEffect(EffectField baseEffect, EffectField scaleEffect, int scaleLevel)
        {
            int finalInitialAmount = baseEffect.m_InitialAmount + Mathf.RoundToInt(scaleEffect.m_InitialAmount * scaleLevel - 1);
            float finalMultiplierAmount = baseEffect.m_MultiplierAmount + (scaleEffect.m_MultiplierAmount * scaleLevel - 1);
            return new EffectField
            {
                m_InitialAmount = finalInitialAmount,
                m_MultiplierAmount = finalMultiplierAmount,
            };
        }
    }
    [System.Serializable]
    public partial class EffectCalculatorField
    {
        [SerializeField]
        private ScalingStat m_ScaleBy = ScalingStat.None;
        [SerializeField]
        private EffectField m_BaseAmount;
        [SerializeField]
        private EffectField m_ScaleByLevel;
        public ScalingStat ScaleBy => m_ScaleBy;
        public EffectField BaseAmount => m_BaseAmount;
        public EffectField ScaleByLevel => m_ScaleByLevel;
    }
}
