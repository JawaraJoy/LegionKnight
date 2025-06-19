using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class HealStat
    {
        [SerializeField]
        private int m_HealAmount = 10;
        [SerializeField]
        private int m_HealAmountUpgrade = 2;
        public int HealAmount => m_HealAmount;
        public int HealAmountUpgrade => m_HealAmountUpgrade;
        public int GetFinalHealAmount(int level)
        {
            return m_HealAmount + m_HealAmountUpgrade * (level - 1);
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
    }
}
