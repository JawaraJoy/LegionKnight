using UnityEngine;

namespace Rush
{
    public static class DamageUtility
    {
        private const int MinimumDamage = 1;

        public static int CalculateRawDamage(AttackerField attacker, IDamageable target)
        {
            int baseDamage = Mathf.RoundToInt(attacker.Attack);

            if (attacker.DamageBasedTargetMaxHP > 0f)
            {
                baseDamage += Mathf.RoundToInt(target.MaxHealth * attacker.DamageBasedTargetMaxHP);
            }

            int result = 0;

            switch (attacker.Type)
            {
                case DamageType.CompareWithDefense:
                    result = CalculateDefenseDamage(baseDamage, target.Defense);
                    break;

                case DamageType.TrueDamage:
                    result = baseDamage;
                    break;

                case DamageType.FatalDamage:
                    result = target.Health + target.Shield;
                    break;
            }

            return Mathf.Max(MinimumDamage, result);
        }

        public static int ApplyCriticalDamage(int damage, AttackerField attacker)
        {
            if (!attacker.IsCritical)
                return damage;

            float bonusDamage = damage * attacker.CriticalDamageRate + attacker.CriticalDamageFlat;
            int finalDamage = Mathf.RoundToInt(damage + bonusDamage);

            return Mathf.Max(MinimumDamage, finalDamage);
        }

        private static int CalculateDefenseDamage(int attack, int defense)
        {
            if (attack <= 0)
                return MinimumDamage;

            float damage = (float)(attack * attack) / (attack + Mathf.Max(0, defense));

            return Mathf.Max(MinimumDamage, Mathf.RoundToInt(damage));
        }

        public static int ApplyDamageReduction(int damage, float reductionRate)
        {
            reductionRate = Mathf.Clamp01(reductionRate);

            int reduced = Mathf.RoundToInt(damage * (1f - reductionRate));

            return Mathf.Max(MinimumDamage, reduced);
        }
    }
}