using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class DamageStat
    {
        [SerializeField]
        private bool m_IsHeroScale = false; // Indicates if the damage stat is scaled with hero stats
        [SerializeField]
        private int m_Attack;
        [SerializeField]
        private int m_Health;

        [SerializeField]
        private int m_AttackUpgrade = 1;
        [SerializeField]
        private int m_HealthUpgrade = 1;
        public int Attack => m_Attack;
        public int Health => m_Health;
        public int AttackUpgrade => m_AttackUpgrade;
        public int HealthUpgrade => m_HealthUpgrade;
        public int GetFinalAttack(int level)
        {
            if (m_IsHeroScale)
            {
                return GetFinalAttackHeroScaleInternal(level);
            }
            else
            {
                return GetFinalAttackInternal(level);
            }
        }
        private int GetFinalAttackInternal(int level)
        {
            return m_Attack + m_AttackUpgrade * (level - 1);
        }

        private int GetFinalAttackHeroScaleInternal(int level)
        {
            CharacterUnit unit = GetUsedCharacterUnit(); // Get the character unit based on the current player character
            int heroAttack = unit.FinalStat(unit.Star, level).Attack;
            return heroAttack + m_Attack + m_AttackUpgrade * (level - 1);
        }

        private CharacterUnit GetUsedCharacterUnit()
        {
            CharacterDefinition characterDefinition = Player.Instance.UsedCharacter; // Get the character definition from the player instance
            return Player.Instance.GetCharacterUnit(characterDefinition);
        }
        public int GetFinalHealth(int level)
        {
            return m_Health + m_HealthUpgrade * (level - 1);
        }
        public void SetAttack(int attack)
        {
            m_Attack = attack;
        }
        public void SetHealth(int health)
        {
            m_Health = health;
        }
        public void SetAttackUpgrade(int attackUpgrade)
        {
            m_AttackUpgrade = attackUpgrade;
        }
        public void SetHealthUpgrade(int healthUpgrade)
        {
            m_HealthUpgrade = healthUpgrade;
        }
        public DamageStat(int attack, int health, int attackUpgrade = 1, int healthUpgrade = 1)
        {
            m_Attack = attack;
            m_Health = health;
            m_AttackUpgrade = attackUpgrade;
            m_HealthUpgrade = healthUpgrade;
        }
    }

    public partial class AbilityDefinition
    {
        [SerializeField]
        private DamageStat m_DamageStat;
        public int Attack => m_DamageStat.Attack;
        public int Health => m_DamageStat.Health;
        public int AttackUpgrade => m_DamageStat.AttackUpgrade;
        public int HealthUpgrade => m_DamageStat.HealthUpgrade;
        public int GetFinalAttack(int level)
        {
            return m_DamageStat.GetFinalAttack(level);
        }
        public int GetFinalHealth(int level)
        {
            return m_DamageStat.GetFinalHealth(level);
        }
    }

    public partial class Damageable
    {
        public void InitStat(AbilityDefinition damageStat)
        {
            m_Damage = damageStat.Attack;
            m_Health = damageStat.Health;
            m_CurrentHealth = m_Health;
        }

        public void InitStat(CharacterDefinition defi)
        {
            CharacterUnit unit = Player.Instance.GetCharacterUnit(defi);
            m_Damage = unit.FinalStat(unit.Star, unit.Level).Attack;
            m_Defend = unit.FinalStat(unit.Star, unit.Level).Defense;
            m_Health = unit.FinalStat(unit.Star, unit.Level).Health;
            m_CurrentHealth = m_Health;
        }
    }
}
