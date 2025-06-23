using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public partial class Stat
    {
        [SerializeField]
        private int m_Attack;
        [SerializeField]
        private int m_Defense; 
        [SerializeField]
        private int m_Health;

        public int Attack => m_Attack;
        public int Defense => m_Defense;
        public int Health => m_Health;

        public static Stat GetStatByLevel(Stat basic, Stat eachGain, int level)
        {
            int attack = basic.m_Attack + Mathf.RoundToInt(eachGain.m_Attack * (level - 1));
            int defense = basic.m_Defense + Mathf.RoundToInt(eachGain.m_Defense * (level - 1));
            int health = basic.m_Health + Mathf.RoundToInt(eachGain.m_Health * (level - 1));
            return new Stat(attack,defense, health);
        }
        public static Stat operator +(Stat a, Stat b)
        {
            return new Stat(a.m_Attack + b.m_Attack, a.m_Defense + b.m_Defense, a.m_Health + b.m_Health);
        }
        public void SetStat(Stat other)
        {
            m_Attack = other.m_Attack;
            m_Defense = other.m_Defense;
            m_Health = other.m_Health;
        }
        public void SetAttack(int value)
        {
            m_Attack = value;
        }
        public void SetDefense(int value)
        {
            m_Defense = value;
        }
        public void SetHealth(int value)
        {
            m_Health = value;
        }
        public Stat(int attack, int def, int health)
        {
            m_Attack = attack;
            m_Defense = def;
            m_Health = health;
        }
    }
}
