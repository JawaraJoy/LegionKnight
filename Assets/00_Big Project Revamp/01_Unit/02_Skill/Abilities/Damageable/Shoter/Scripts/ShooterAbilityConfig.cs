
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Shot Ability", menuName = "Rush/Combat/Ability/Shot")]
    public class ShooterAbilityConfig : DamageAbilityConfig
    {
        [SerializeField]
        private SpawnShapeConfig m_SpawnShape;
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
