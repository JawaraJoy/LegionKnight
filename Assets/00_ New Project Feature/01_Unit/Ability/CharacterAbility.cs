using Rush;
using UnityEngine;

namespace Rush
{
    public class CharacterAbility { }

    public partial class CharacterConfig
    {
        [SerializeField]
        private AbilityConfig[] m_Abilities;
        public AbilityConfig[] Abilities => m_Abilities;
    }
}
