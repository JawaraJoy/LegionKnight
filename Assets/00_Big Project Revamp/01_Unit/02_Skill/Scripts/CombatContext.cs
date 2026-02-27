using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public partial class CombatContext
    {
        private readonly IHasAttacker m_Attacker;
        private readonly IDamageable m_Damageable;
        public IHasAttacker Attacker => m_Attacker;
        public IDamageable Damageable => m_Damageable;
        public CombatContext(IHasAttacker attacker, IDamageable damageable)
        {
            m_Attacker = attacker;
            m_Damageable = damageable;
        }
    }
}
