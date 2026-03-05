
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Shot Ability", menuName = "Rush/Combat/Ability/Shot")]
    public class ShooterAbilityConfig : DamageAbilityConfig
    {
        [SerializeField]
        private SpawnSetupField m_SpawningSetup;
        public SpawnSetupField SpawningSetup => m_SpawningSetup;
        
        [SerializeField]
        private SpawnShapeConfig m_SpawnShape;

        [Header("Movement & Targeting")]
        [SerializeField]
        private bool m_ShoterLookAtTargetOnActivate;
        
        [SerializeField]
        private TargetDistributeMode m_TargetDistributeMode;
        
        
        public TargetDistributeMode TargetDistributeMode => m_TargetDistributeMode;
        public bool ShoterLookAtTargetOnActivate => m_ShoterLookAtTargetOnActivate;
        public SpawnShapeConfig SpawnShape => m_SpawnShape;


        protected override int GetDamageInternal(IAbilityContext context)
        {
            float damage = AbilityUltility.GetFinalPowerAmount(context);
            return Mathf.RoundToInt(damage);
        }
    }

    public enum TargetingMode
    {
        None = 0,
        Straight = 1,
        Homing = 2,
    }
    
}
