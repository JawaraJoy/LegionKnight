using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class HealStat
    {
        [SerializeField]
        private int m_HealAmount = 10;
        [SerializeField]
        private float m_HealRate = 1.0f; // Heal rate per second, can be adjusted in the inspector
        [SerializeField]
        private int m_HealAmountUpgrade = 2;
        [SerializeField]
        private float m_HealRateUpgrade = 0.1f; // Upgrade rate for heal per second, can be adjusted in the inspector
        public int HealAmount => m_HealAmount;
        public int HealAmountUpgrade => m_HealAmountUpgrade;
        public int GetFinalHealAmount(int level)
        {
            int finalHealAmount = m_HealAmount + m_HealAmountUpgrade * (level - 1);
            return finalHealAmount;
        }
        public float GetFinalHealRate(int level)
        {
            float finalHealRate = m_HealRate + m_HealRateUpgrade * (level - 1);
            return finalHealRate;
        }
        public void SetHealAmount(int healAmount)
        {
            m_HealAmount = healAmount;
        }
        public void SetHealAmountUpgrade(int healAmountUpgrade)
        {
            m_HealAmountUpgrade = healAmountUpgrade;
        }
        public HealStat(int healAmount, int upgrade)
        {
            m_HealAmount = healAmount;
            m_HealAmountUpgrade = upgrade;
        }
    }

    public partial class AbilityDefinition
    {
        [SerializeField]
        private HealStat m_HealStat;
        public int HealAmount => m_HealStat.HealAmount;
        public int HealAmountUpgrade => m_HealStat.HealAmountUpgrade;
        public int GetFinalHealAmount(int level)
        {
            return m_HealStat.GetFinalHealAmount(level);
        }
        public float GetFinalHealRate(int level)
        {
            return m_HealStat.GetFinalHealRate(level);
        }
    }
}
