using UnityEngine;

namespace Rush
{
    public static class DamageUtility
    {
        private const int m_MinimumDefendReduction = 0;
        public static int DamageFormulaRPG(IAttacker attacker, IDamageable damageable)
        {
            int baseDamage = attacker.Damage;
            float damageBaseMaxHp = damageable.MaxHealth * attacker.DamageBasedTargetMaxHP;
            int damageBaseMaxHpRounded = Mathf.RoundToInt(damageBaseMaxHp);
            baseDamage += damageBaseMaxHpRounded;

            int def = Mathf.RoundToInt(damageable.Defense);
            int comparedDef = Mathf.Clamp(baseDamage + def, m_MinimumDefendReduction, int.MaxValue);
            int calculatedDamage = Mathf.RoundToInt(baseDamage * baseDamage / (comparedDef));

            if (calculatedDamage < 1)
            {
                calculatedDamage = 1;
            }

            if (attacker.IsTrueDamage)
            {
                calculatedDamage = baseDamage;
            }
            int finalDamage = calculatedDamage - Mathf.RoundToInt(calculatedDamage * damageable.DamageReductionRate);
            return finalDamage;
        }
    }
}
