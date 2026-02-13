using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public partial class BattleContext
    {
        private readonly IAttacker m_Attacker;
        private readonly IDamageable m_Damageable;
        public IAttacker Attacker => m_Attacker;
        public IDamageable Damageable => m_Damageable;
        public BattleContext(IAttacker attacker, IDamageable damageable)
        {
            m_Attacker = attacker;
            m_Damageable = damageable;
        }
    }
}
