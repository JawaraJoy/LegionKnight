using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class PowerField
    {
        [SerializeField]
        private int m_InitialAmount;
        [SerializeField]
        private float m_MultiplierAmount;
        public int InitialAmount => m_InitialAmount;
        public float MultiplierAmount => m_MultiplierAmount;
        public PowerField Zero
        {
            get
            {
                return new PowerField
                {
                    m_InitialAmount = 0,
                    m_MultiplierAmount = 0f,
                };
            }
        }

        public static PowerField operator+(PowerField a, PowerField b)
        {
            return new PowerField
            {
                m_InitialAmount = a.m_InitialAmount + b.m_InitialAmount,
                m_MultiplierAmount = a.m_MultiplierAmount + b.m_MultiplierAmount,
            };
        }
        public static PowerField operator -(PowerField a, PowerField b)
        {
            return new PowerField
            {
                m_InitialAmount = a.m_InitialAmount - b.m_InitialAmount,
                m_MultiplierAmount = a.m_MultiplierAmount - b.m_MultiplierAmount,
            };
        }
        public static PowerField operator*(PowerField a, PowerField b)
        {
            return new PowerField
            {
                m_InitialAmount = (int)(a.m_InitialAmount * b.m_InitialAmount),
                m_MultiplierAmount = a.m_MultiplierAmount * b.m_MultiplierAmount,
            };
        }
        public static PowerField operator*(PowerField a, int scalar)
        {
            return new PowerField
            {
                m_InitialAmount = a.m_InitialAmount * scalar,
                m_MultiplierAmount = a.m_MultiplierAmount * scalar,
            };
        }

        public static PowerField GetFinalPower(PowerField baseEffect, PowerField scaleEffect, int scaleLevel)
        {
            int finalInitialAmount = baseEffect.m_InitialAmount + Mathf.RoundToInt(scaleEffect.m_InitialAmount * scaleLevel);
            float finalMultiplierAmount = baseEffect.m_MultiplierAmount + (scaleEffect.m_MultiplierAmount * scaleLevel);
            return new PowerField
            {
                m_InitialAmount = finalInitialAmount,
                m_MultiplierAmount = finalMultiplierAmount,
            };
        }
    }
    [System.Serializable]
    public partial class AbilityPowerField
    {
        [SerializeField] 
        PowerPurpose m_Purpose = PowerPurpose.Damage;
        [SerializeField]
        private ScalingWithStat m_ScaleBy = ScalingWithStat.None;
        [SerializeField]
        private PowerField m_BaseAmount;
        [SerializeField]
        private PowerField m_ScaleByLevel;
        public PowerPurpose Purpose => m_Purpose;
        public ScalingWithStat ScaleBy => m_ScaleBy;
        public PowerField BaseAmount => m_BaseAmount;
        public PowerField ScaleByLevel => m_ScaleByLevel;
    }

    public enum PowerPurpose
    {
        Damage,
        Heal,
        Shield,
        StatModifier,
    }
}
