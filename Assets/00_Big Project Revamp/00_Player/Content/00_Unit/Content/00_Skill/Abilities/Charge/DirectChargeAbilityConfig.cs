using LegionKnight;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Charge Ability", menuName = "Rush/Combat/Ability/Direct Charge")]
    public class DirectChargeAbilityConfig : ChargeAbilityConfig
    {
        [SerializeField]
        private SpawnSetupField m_SpawnSetup;
        public SpawnSetupField SpawningSetup => m_SpawnSetup;
        [SerializeField]
        private TargetDistributeMode m_TargetDistributeMode;
        public TargetDistributeMode TargetDistributeMode => m_TargetDistributeMode;
    }
}
