using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Defensive", menuName = "Rush/Combat/StatusEff/Defensive", order = 1)]
    public class DefensiveStatusEffectConfig : StatusEffectConfig
    {
        [Header("Sources")]
        [SerializeField]
        private int m_RebornCount;
        [SerializeField]
        private int m_Shield;
        [SerializeField]
        private float m_ShieldBasedDefendRate;
        [SerializeField]
        private int m_Barrier;

        [Header("Stats")]
        [SerializeField]
        private float m_DamageReductionRate;

        [Header("States")]
        [SerializeField]
        private bool m_Immortality;
        [SerializeField]
        private bool m_Invisibility;
        public override void ApplyEffect(StatusEffectContext context)
        {
            if (context.Infected.HasBind(out Damageable damageable))
            {
                if (m_Invisibility)
                {
                    damageable.SetInvisible(true);
                }
                if (m_Immortality)
                {
                    damageable.SetImmortal(true);
                }
            }
        }

        public override void DoneEffect(StatusEffectContext context)
        {
            if (context.Infected.HasBind(out Damageable damageable))
            {
                if (m_Invisibility)
                {
                    damageable.SetInvisible(false);
                }
                if (m_Immortality)
                {
                    damageable.SetImmortal(false);
                }
            }
        }
        public override void OnStackAdded(StatusEffectContext context)
        {
            if (context.Infected.HasBind(out Damageable damageable))
            {
                damageable.AddRemainingRebornCount(m_RebornCount, true);
                damageable.AddShield(m_Shield, true);
                damageable.AddShieldBasedOnDefendRate(m_ShieldBasedDefendRate, true);
                damageable.AddBarrier(m_Barrier, true);
                damageable.AddDamageReductionRate(m_DamageReductionRate);
            }
        }
        public override void OnStackRemoved(StatusEffectContext context)
        {
            if (context.Infected.HasBind(out Damageable damageable))
            {
                damageable.AddRemainingRebornCount(-m_RebornCount, false);
                damageable.AddShield(-m_Shield, false);
                damageable.AddShieldBasedOnDefendRate(-m_ShieldBasedDefendRate, false);
                damageable.AddBarrier(-m_Barrier, false);
                damageable.AddDamageReductionRate(-m_DamageReductionRate);
            }
        }
    }
}
