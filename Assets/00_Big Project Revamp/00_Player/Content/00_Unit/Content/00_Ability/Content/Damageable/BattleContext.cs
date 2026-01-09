using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class BattleContext
    {
        [SerializeField, MMReadOnly]
        private Attacker m_Attacker;
        [SerializeField, MMReadOnly]
        private Damageable m_Damageable;

        private const int m_MinimumDefendReduction = 0;
        public Attacker Attacker => m_Attacker;
        public Damageable Damageable => m_Damageable;
        public BattleContext(Attacker attacker, Damageable damageable)
        {
            m_Attacker = attacker;
            m_Damageable = damageable;
        }

        public int DamageFormulaRPG()
        {
            int atk = m_Attacker.Damage;
            int def = Mathf.RoundToInt(m_Damageable.Defense);
            int underAmor = Mathf.Clamp(atk + def, m_MinimumDefendReduction, int.MaxValue);
            int dmg = Mathf.RoundToInt(atk * atk / (underAmor));
            if (dmg < 1)
            {
                dmg = 1;
            }
            if (m_Attacker.IsTrueDamage)
            {
                dmg = atk;
            }
            dmg -= Mathf.RoundToInt(dmg * m_Damageable.DamageReductionRate);
            return dmg;
        }
    }
}
