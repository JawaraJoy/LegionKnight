using Rush;
using UnityEngine;

namespace LegionKnight
{
    public partial class BosDamageable : Damageable
    {
        /*protected override void OnContactedBehaviourInvoke(IContactable other)
        {
            base.OnContactedBehaviourInvoke(other);

            
        }*/
    }
    public partial class BosEnemy
    {
        [SerializeField]
        private BosDamageable m_Damageable;
        public BosDamageable Damageable => m_Damageable;
        public void InitDamageable(int healthBonus)
        {
            m_Damageable.Init(0, m_BosDefinition.Health + healthBonus);
        }
    }
}
