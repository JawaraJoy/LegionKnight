using UnityEngine;

namespace Rush
{
    public class Damageable : MonoBehaviour
    {
        [SerializeField]
        private int m_Health = 100;
        [SerializeField]
        private int m_Defense = 0;
        [SerializeField]
        private int m_Shield = 0;
        [SerializeField]
        private int m_Barrier = 0;
        public float Health => m_Health;
        public float Defense => m_Defense;
        public int Shield => m_Shield;
        public int Barrier => m_Barrier;

        private const int m_MinimumDefendReduction = 0;
        private int DamageFormulaRPG(Attacker attacker, Damageable defender)
        {
            int atk = attacker.Damage;
            int def = Mathf.RoundToInt(defender.Defense);
            int underAmor = Mathf.Clamp(atk + def, m_MinimumDefendReduction, int.MaxValue);
            int dmg = Mathf.RoundToInt(atk * atk / (underAmor));
            if (dmg < 1)
            {
                dmg = 1;
            }
            if (attacker.IsTrueDamage)
            {
                dmg = atk;
            }
            return dmg;
        }
        public void Init(AbilityContext context)
        {
            int level = context.Owner.Progression.Level;
            float healthFinal = Mathf.Max(0f, context.Owner.Config.MainStats.GetFinalStat(level).Health);
            float defenseFinal = Mathf.Max(0f, context.Owner.Config.MainStats.GetFinalStat(level).Defense);
            m_Health = Mathf.RoundToInt(healthFinal);
            m_Defense = Mathf.RoundToInt(defenseFinal);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Attacker attacker))
            {
                TakeDamageInternal(attacker);
            }
        }
        protected virtual void TakeDamageInternal(Attacker attacker)
        {
            int effectiveDamage = DamageFormulaRPG(attacker, this);
            // Apply damage to Barrier first
            if (m_Barrier > 0)
            {
                AddBarrierInternal(-1);
                return;
            }
            // Apply remaining damage to Shield
            if (effectiveDamage > 0f && m_Shield > 0)
            {
                int previousShield = m_Shield;
                AddShieldInternal(-effectiveDamage);
                if (m_Shield < 0)
                {
                    effectiveDamage = -previousShield;
                    m_Shield = 0;
                }
                else
                {
                    effectiveDamage = 0;
                }
            }
            // Apply remaining damage to Health
            if (effectiveDamage > 0f)
            {
                AddHealthInternal(-effectiveDamage);
            }
        }
        protected virtual void AddHealthInternal(int amount)
        {
            m_Health += amount;
            m_Health = Mathf.Max(0, m_Health);
        }
        protected virtual void AddDefenseInternal(int amount)
        {
            m_Defense += amount;
            m_Defense = Mathf.Max(m_MinimumDefendReduction, m_Defense);
        }
        protected virtual void AddShieldInternal(int amount)
        {
            m_Shield += amount;
            m_Shield = Mathf.Max(0, m_Shield);
        }
        protected virtual void AddBarrierInternal(int amount)
        {
            m_Barrier += amount;
            m_Barrier = Mathf.Max(0, m_Barrier);
        }
    }
}
