using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Rush/Combat/Ability", order = 1)]
    public partial class AbilityConfig : Configuration
    {
        [SerializeField]
        private ProgressField m_LevelSet;
        [SerializeField]
        private AbilitySetUpField[] m_AbilitySets;
        public ProgressField LevelSet => m_LevelSet;
        public AbilitySetUpField[] AbilitySets => m_AbilitySets;
        public AbilitySetUpField GetAbilitySetUp(AbilityPurpose purpose)
        {
            foreach (var abilitySet in m_AbilitySets)
            {
                if (abilitySet.Purpose == purpose)
                {
                    return abilitySet;
                }
            }
            return null;
        }
    }
}
