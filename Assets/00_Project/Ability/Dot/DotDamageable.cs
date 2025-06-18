using UnityEngine;

namespace LegionKnight
{
    public class DotDamageable : Damageable
    {
        [SerializeField]
        private float m_Duration = 5f;

        protected override void OnContactedBehaviourInvoke(GameObject other)
        {
            base.OnContactedBehaviourInvoke(other);
            if (other.TryGetComponent(out DamageOvertime projectile))
            {
                projectile.ApplyDamageOverTime(m_Damage, m_Duration);
            }
        }
        public void Initialize(AbilityDefinition defi, int level)
        {
            // Initialize the ability with the provided definition
            // This can include setting up specific properties or configurations based on the definition
            if (defi != null)
            {
                m_Damage = defi.GetFinalDotDamagePerTick(level);
                m_Health = defi.GetFinalHealth(level);
                m_Duration = defi.GetFinalDotDuration(level);
            }
        }
    }
}
