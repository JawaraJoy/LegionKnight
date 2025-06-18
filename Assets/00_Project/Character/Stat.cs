using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public partial class Stat
    {
        [SerializeField]
        private int m_Attack;
        [SerializeField]
        private int m_Health;

        public int Attack => m_Attack;
        public int Health => m_Health;

        public Stat GetStatByLevel(Stat eachGain, int level)
        {
            m_Attack += Mathf.RoundToInt(eachGain.m_Attack * level);
            m_Health += Mathf.RoundToInt(eachGain.m_Health * level);
            return this;
        }
        public void SetStat(Stat other)
        {
            m_Attack = other.m_Attack;
            m_Health = other.m_Health;
        }
        public void SetAttack(int value)
        {
            m_Attack = value;
        }
        public void SetHealth(int value)
        {
            m_Health = value;
        }
    }
}
