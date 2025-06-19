using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class RegenStat
    {
        [SerializeField]
        private int m_RegenAmount = 5;
        [SerializeField]
        private float m_RegenDuration = 5f;
        [SerializeField]
        private int m_RegenAmountUpgrade = 1;
        [SerializeField]
        private float m_RegenDurationUpgrade = 0.5f;
        public float RegenDuration => m_RegenDuration;
        public int RegenAmount => m_RegenAmount;

        public int RegenAmountUpgrade => m_RegenAmountUpgrade;
        public float RegenDurationUpgrade => m_RegenDurationUpgrade;

        public int GetFinalRegenAmount(int level)
        {
            return m_RegenAmount + m_RegenAmountUpgrade * (level - 1);
        }
        public float GetFinalRegenDuration(int level)
        {
            return m_RegenDuration + m_RegenDurationUpgrade * (level - 1);
        }
        public void SetRegenAmount(int regenAmount)
        {
            m_RegenAmount = regenAmount;
        }
        public void SetRegenDuration(float regenDuration)
        {
            m_RegenDuration = regenDuration;
        }
        public void SetRegenAmountUpgrade(int regenAmountUpgrade)
        {
            m_RegenAmountUpgrade = regenAmountUpgrade;
        }
        public void SetRegenDurationUpgrade(float regenDurationUpgrade)
        {
            m_RegenDurationUpgrade = regenDurationUpgrade;
        }
        public RegenStat(int regenAmount, float regenDuration, int regenAmountUpgrade, float regenDurationUpgrade)
        {
            m_RegenAmount = regenAmount;
            m_RegenDuration = regenDuration;
            m_RegenAmountUpgrade = regenAmountUpgrade;
            m_RegenDurationUpgrade = regenDurationUpgrade;
        }
    }

    public partial class AbilityDefinition
    {
        [SerializeField]
        private RegenStat m_RegenStat;
        public int RegenAmount => m_RegenStat.RegenAmount;
        public float RegenDuration => m_RegenStat.RegenDuration;
        public int RegenAmountUpgrade => m_RegenStat.RegenAmountUpgrade;
        public float RegenDurationUpgrade => m_RegenStat.RegenDurationUpgrade;
        public int GetFinalRegenAmount(int level)
        {
            return m_RegenStat.GetFinalRegenAmount(level);
        }
        public float GetFinalRegenDuration(int level)
        {
            return m_RegenStat.GetFinalRegenDuration(level);
        }
    }
}
