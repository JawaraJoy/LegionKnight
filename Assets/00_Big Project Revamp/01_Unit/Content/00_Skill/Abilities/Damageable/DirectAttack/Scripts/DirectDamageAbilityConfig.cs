using LegionKnight;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Direct Damage", menuName = "Rush/Combat/Ability/DirectDamage")]
    public class DirectDamageAbilityConfig : DamageAbilityConfig
    {
        [SerializeField]
        private SpawnSetupField m_SpawningSetup;
        public SpawnSetupField SpawningSetup => m_SpawningSetup;
        [SerializeField]
        private ExplodeSetupField m_ExplodeSetup;
        public ExplodeSetupField ExplodeSetup => m_ExplodeSetup;
        [SerializeField]
        private TargetDistributeMode m_TargetDistributeMode;
        [SerializeField]
        private float m_AttackDelay = 0f;
        public float AttackDelay => m_AttackDelay;
        public TargetDistributeMode TargetDistributeMode => m_TargetDistributeMode;
        protected override float GetDamage(AbilityContext context)
        {
            return AbilityUltility.GetFinalEffectAmount(context);
        }
    }
}
