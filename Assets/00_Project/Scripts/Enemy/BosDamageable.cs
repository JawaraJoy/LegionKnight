using Rush;
using UnityEngine;

namespace LegionKnight
{
    public partial class BosDamageable : Damageable
    {
        protected override void OnContactedBehaviourInvoke(IContactable other)
        {
            base.OnContactedBehaviourInvoke(other);

            if (other is ProjectileDamage projectile)
            {
                TakeDamageInternal(projectile.Damage);
                Destroy(projectile.gameObject);
            }
        }
    }
    public partial class BosEnemy
    {
        [SerializeField]
        private BosDamageable m_Damageable;

        public void InitDamageable()
        {
            m_Damageable.Init(0, m_BosDefinition.Health);
        }
    }
}
