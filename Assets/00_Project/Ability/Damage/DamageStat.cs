using System.Collections;
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
        private float m_AttackRate = 1.0f; // Attack rate
        [SerializeField]
        private int m_Health;
        [SerializeField]
        private int m_Barrier;

        [SerializeField]
        private int m_AttackUpgrade = 1;
        [SerializeField]
        private float m_AttackRateUpgrade = 0.1f; // Attack rate upgrade
        [SerializeField]
        private int m_HealthUpgrade = 1;
        [SerializeField]
        private int m_ShieldUpgrade = 1;
        public int Attack => m_Attack;
        public float AttackRate => m_AttackRate;
        public int Health => m_Health;
        public int AttackUpgrade => m_AttackUpgrade;
        public float AttackRateUpgrade => m_AttackRateUpgrade;
        public int HealthUpgrade => m_HealthUpgrade;
        public int Barrier => m_Barrier;
        public int ShieldUpgrade => m_ShieldUpgrade;

        private float AttackRateRule
        {
            get
            {
                float rate;
                if ( m_AttackRate <= 0f)
                {
                    rate = 1f;
                }
                else
                {
                    rate = m_AttackRate;
                }
                return rate;
            }
        }
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
            int finalAttackUpgrade = m_Attack + m_AttackUpgrade * (level - 1);
            float finalAttackRate = AttackRateRule + m_AttackRateUpgrade * (level - 1);
            int scaledAttack = Mathf.RoundToInt(finalAttackUpgrade * finalAttackRate);
            return scaledAttack;
        }

        private PlayerDamageBuff m_DamageBuff;
        private PlayerDamageBuff GetPlayerDamageBuff()
        {
            if (m_DamageBuff == null)
            {
                m_DamageBuff = Player.Instance.GetPlayerDamageBuff();
            }
            return m_DamageBuff;
        }
        private int GetFinalAttackHeroScaleInternal(int level)
        {
            CharacterUnit unit = GetUsedCharacterUnit(); // Get the character unit based on the current player character
            int heroAttack = unit.FinalStat().Attack;
            int buffAttack = GetPlayerDamageBuff().GetDamageStat().GetFinalAttack(1);
            int finalAttackUpgrade = m_Attack + buffAttack + m_AttackUpgrade * (level - 1);
            float finalAttackRate = AttackRateRule + m_AttackRateUpgrade * (level - 1);
            int scaledHeroAttack = Mathf.RoundToInt((heroAttack + finalAttackUpgrade) * finalAttackRate);
            Debug.Log($"{scaledHeroAttack} Final Attack Hero");
            return scaledHeroAttack;
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
        public int GetFinalShield(int level)
        {
            return (m_Barrier + m_ShieldUpgrade * (level - 1));
        }
        public void SetAttack(int attack)
        {
            m_Attack = attack;
        }
        public void SetAttackRate(float attackRate)
        {
            m_AttackRate = attackRate;
        }
        public void AddAttackRate(float attackRate)
        {
            m_AttackRate += attackRate;
        }
        public IEnumerator AddAttackRateTemping(float attackRate, float duration)
        {
            m_AttackRate += attackRate;
            yield return new WaitForSeconds(duration);
            m_AttackRate -= attackRate;
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
        public int Barrier => m_DamageStat.Barrier;
        public int AttackUpgrade => m_DamageStat.AttackUpgrade;
        public int HealthUpgrade => m_DamageStat.HealthUpgrade;
        public int ShieldUpgrade => m_DamageStat.ShieldUpgrade;
        public int GetFinalAttack(int level)
        {
            return m_DamageStat.GetFinalAttack(level);
        }
        public int GetFinalHealth(int level)
        {
            return m_DamageStat.GetFinalHealth(level);
        }
        public int GetFinalShield(int level)
        {
            return m_DamageStat.GetFinalShield(level);
        }
    }

    public partial class Damageable
    {
        public void InitStat(AbilityDefinition damageStat)
        {
            if (damageStat == null) return;
            CharacterDefinition characterDefinition = Player.Instance.UsedCharacter; // Get the character definition from the player instance
            CharacterUnit unit = Player.Instance.GetCharacterUnit(characterDefinition);
            int level = unit.Level;
            m_Damage = damageStat.GetFinalAttack(level);
            m_Health = damageStat.GetFinalHealth(level);
            m_CurrentHealth = m_Health;
            m_Barrier = damageStat.Barrier;
        }

        public void InitStat(CharacterDefinition defi)
        {
            if (defi == null) return;
            CharacterUnit unit = Player.Instance.GetCharacterUnit(defi);
            m_Damage = unit.FinalStat().Attack;
            m_Defend = unit.FinalStat().Defense;
            m_Health = unit.FinalStat().Health;
            m_CurrentHealth = m_Health;
        }
    }
}
