using UnityEngine;

namespace Rush
{
    public static class DamageUtility
    {
        private const int m_MinimumDefendReduction = 0;
        public static int DamageFormulaRPG(IHasAttacker attacker, IDamageable damageable)
        {
            int baseDamage = Mathf.RoundToInt(attacker.AttackerField.Attack);
            float damageBaseMaxHp = damageable.MaxHealth * attacker.AttackerField.DamageBasedTargetMaxHP;
            int damageBaseMaxHpRounded = Mathf.RoundToInt(damageBaseMaxHp);
            baseDamage += damageBaseMaxHpRounded;

            int reducedDamage;
            int finalDamage = 0;
            switch (attacker.AttackerField.Type)
            {
                case DamageType.CompareWithDefense:
                    //Compare with Defend
                    int def = Mathf.RoundToInt(Mathf.RoundToInt(damageable.Defense));
                    int comparedDef = Mathf.Clamp(baseDamage + def, m_MinimumDefendReduction, int.MaxValue);
                    int calculatedDamage = Mathf.RoundToInt(baseDamage * baseDamage / (comparedDef));

                    reducedDamage = Mathf.RoundToInt(calculatedDamage * damageable.DamageReductionRate);

                    finalDamage = calculatedDamage - reducedDamage;
                    break;
                case DamageType.TrueDamage:
                    reducedDamage = Mathf.RoundToInt(baseDamage * damageable.DamageReductionRate);
                    finalDamage = baseDamage - reducedDamage;
                    break;
                case DamageType.FatalDamage:
                    finalDamage = Mathf.RoundToInt(damageable.Health) + damageable.Shield;
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
