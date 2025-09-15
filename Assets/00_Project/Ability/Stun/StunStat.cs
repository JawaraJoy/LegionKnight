using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class StunStat
    {
        [SerializeField]
        private float m_StunDuration = 2f;
        [SerializeField]
        private float m_StunDurationUpgrade = 0.5f;
        public float StunDuration => m_StunDuration;
        public float StunDurationUpgrade => m_StunDurationUpgrade;
        public float GetFinalStunDuration(int level)
        {
            return m_StunDuration + m_StunDurationUpgrade * (level - 1);
        }
        public void SetStunDuration(float stunDuration)
        {
            m_StunDuration = stunDuration;
        }
        public void SetStunDurationUpgrade(float stunDurationUpgrade)
        {
            m_StunDurationUpgrade = stunDurationUpgrade;
        }
        public StunStat(float stunDuration, float stunDurationUpgrade)
        {
            m_StunDuration = stunDuration;
            m_StunDurationUpgrade = stunDurationUpgrade;
        }
    }

    public partial class AbilityDefinition
    {
        [SerializeField]
        private StunStat m_StunStat;
        public float StunDuration => m_StunStat.StunDuration;
        public float StunDurationUpgrade => m_StunStat.StunDurationUpgrade;
        public float GetFinalStunDuration(int level)
        {
            return m_StunStat.GetFinalStunDuration(level);
        }
    }
}
