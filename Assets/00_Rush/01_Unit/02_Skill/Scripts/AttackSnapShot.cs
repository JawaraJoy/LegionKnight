using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public struct AttackSnapshot
    {
        public int AttackPower;
        public float DamageBasedTargetMaxHP;
        public DamageType DamageType;

        public bool CanCriticalHit;
        public float CriticalChance;
        public float CriticalDamageFlat;
        public float CriticalDamageRate;

        public AttackSnapshot(
            int attackPower,
            float damageBasedTargetMaxHP,
            DamageType damageType,
            bool canCriticalHit,
            float criticalChance,
            float criticalDamageFlat,
            float criticalDamageRate)
        {
            AttackPower = attackPower;
            DamageBasedTargetMaxHP = damageBasedTargetMaxHP;
            DamageType = damageType;
            CanCriticalHit = canCriticalHit;
            CriticalChance = criticalChance;
            CriticalDamageFlat = criticalDamageFlat;
            CriticalDamageRate = criticalDamageRate;
        }
    }
}