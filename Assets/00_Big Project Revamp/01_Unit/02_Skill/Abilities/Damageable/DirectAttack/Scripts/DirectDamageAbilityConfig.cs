using LegionKnight;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Direct Damage", menuName = "Rush/Combat/Ability/DirectDamage")]
    public class DirectDamageAbilityConfig : DamageAbilityConfig
    {
        [SerializeField]
        private ExplodeSetupField m_ExplodeSetup;
        public ExplodeSetupField ExplodeSetup => m_ExplodeSetup;
        
        [SerializeField]
        private float m_AttackDelay = 0f;
        [SerializeField]
        private float m_DecayDelay = 0f;
        public float AttackDelay => m_AttackDelay;
        public float DecayDelay => m_DecayDelay;
        
        protected override int GetDamageInternal(IAbilityContext context)
        {
            float damage = AbilityUltility.GetFinalPowerAmount(context);
            return Mathf.RoundToInt(damage);
        }
    }
}
