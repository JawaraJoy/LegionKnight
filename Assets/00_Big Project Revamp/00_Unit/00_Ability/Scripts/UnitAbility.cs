using Rush;
using UnityEngine;

namespace Rush
{
    public class UnitAbility { }

    public partial class UnitConfig
    {
        [SerializeField]
        private AbilityConfig[] m_Abilities;
        public AbilityConfig[] Abilities => m_Abilities;
    }
}
