using LegionKnight;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Heal Ability", menuName = "Rush/Combat/Ability/Direct Heal")]
    public class DirectHealAbilityConfig : HealAbilityConfig
    {
        [SerializeField]
        private SpawnSetupField m_SpawnSetup;
        public SpawnSetupField SpawningSetup => m_SpawnSetup;
        [SerializeField]
        private TargetDistributeMode m_TargetDistributeMode;
        public TargetDistributeMode TargetDistributeMode => m_TargetDistributeMode;
    }
}
