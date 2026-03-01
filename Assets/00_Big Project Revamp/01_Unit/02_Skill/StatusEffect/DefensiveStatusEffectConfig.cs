using UnityEngine;
using UnityEngine.Purchasing;

namespace Rush
{
    [CreateAssetMenu(fileName = "Defensive", menuName = "Rush/Combat/StatusEff/Defensive", order = 1)]
    public class DefensiveStatusEffectConfig : StatusEffectConfig
    {
        [Header("Defensive Status Effect")]
        [SerializeField]
        private int m_RebornCount;
        [SerializeField]
        private int m_Shield;
        [SerializeField]
        private int m_Barrier;
        [SerializeField]
        private float m_DamageReductionRate;
        [SerializeField]
        private bool m_Immortality;
        [SerializeField]
        private bool m_Invisibility;
        public override void ApplyEffect(Unit unitTarget)
        {
            if (unitTarget.HasBind(out Damageable damageable))
            {
                damageable.SetImmortal(m_Immortality);
                damageable.SetInvisible(m_Invisibility);
            }
        }

        public override void DoneEffect(Unit unitTarget)
        {
            if (unitTarget.HasBind(out Damageable damageable))
            {
                damageable.SetImmortal(false);
                damageable.SetInvisible(false);
            }
        }
        public override void OnStackAdded(Unit unitTarget)
        {
            if (unitTarget.HasBind(out Damageable damageable))
            {
                damageable.AddRemainingRebornCount(m_RebornCount, true);
                damageable.AddShield(m_Shield, true);
                damageable.AddBarrier(m_Barrier, true);
                damageable.AddDamageReductionRate(m_DamageReductionRate);
            }
        }
        public override void OnStackRemoved(Unit unitTarget)
        {
            if (unitTarget.HasBind(out Damageable damageable))
            {
                damageable.AddRemainingRebornCount(-m_RebornCount, false);
                damageable.AddShield(-m_Shield, false);
                damageable.AddBarrier(-m_Barrier, false);
                damageable.AddDamageReductionRate(-m_DamageReductionRate);
            }
        }
    }
}
