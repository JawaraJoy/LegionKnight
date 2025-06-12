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
    }
}
