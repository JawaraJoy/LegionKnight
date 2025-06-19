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

        public static Stat GetStatByLevel(Stat basic, Stat eachGain, int level)
        {
            int attack = basic.m_Attack + Mathf.RoundToInt(eachGain.m_Attack * (level - 1));
            int health = basic.m_Health + Mathf.RoundToInt(eachGain.m_Health * (level - 1));
            return new Stat(attack, health);
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
        public Stat(int attack, int health)
        {
            m_Attack = attack;
            m_Health = health;
        }
    }
}
