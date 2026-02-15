using UnityEngine;

namespace Rush
{
    public static class DamageUtility
    {
        private const int m_MinimumDefendReduction = 0;
        public static int DamageFormulaRPG(IAttacker attacker, IDamageable damageable)
        {
            int baseDamage = attacker.AttackerField.Damage;
            float damageBaseMaxHp = damageable.DamageableField.MaxHealth * attacker.AttackerField.DamageBasedTargetMaxHP;
            int damageBaseMaxHpRounded = Mathf.RoundToInt(damageBaseMaxHp);
            baseDamage += damageBaseMaxHpRounded;

            int reducedDamage;
            int finalDamage = 0;
            switch (attacker.AttackerField.Type)
            {
                case DamageType.CompareWithDefense:
                    //Compare with Defend
                    int def = Mathf.RoundToInt(damageable.DamageableField.Defense);
                    int comparedDef = Mathf.Clamp(baseDamage + def, m_MinimumDefendReduction, int.MaxValue);
                    int calculatedDamage = Mathf.RoundToInt(baseDamage * baseDamage / (comparedDef));

                    reducedDamage = Mathf.RoundToInt(calculatedDamage * damageable.DamageableField.DamageReductionRate);

                    finalDamage = calculatedDamage - reducedDamage;
                    break;
                case DamageType.TrueDamage:
                    reducedDamage = Mathf.RoundToInt(baseDamage * damageable.DamageableField.DamageReductionRate);
                    finalDamage = baseDamage - reducedDamage;
                    break;
                case DamageType.FatalDamage:
                    finalDamage = damageable.DamageableField.Health + damageable.DamageableField.Shield;
                    break;
            }
            
            if (finalDamage < 1)
            {
                finalDamage = 1;
            }
            return finalDamage;
        }
    }
}
