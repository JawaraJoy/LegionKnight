using UnityEngine;

namespace LegionKnight
{
    public class DotDamageable : Damageable, ISelfAbility
    {
        [SerializeField]
        private AbilityDefinition m_AbilityDefinition;
        [SerializeField]
        private float m_Duration = 5f;

        protected override void OnContactedBehaviourInvoke(GameObject other)
        {
            base.OnContactedBehaviourInvoke(other);
            if (other.TryGetComponent(out DamageOvertime projectile))
            {
                projectile.ApplyDamageOverTime(m_Damage, m_Duration);
            }
        }
        public void Initialize()
        {
            if (m_AbilityDefinition == null) return; // Ensure the ability definition is set before proceeding
            CharacterDefinition characterDefinition = Player.Instance.UsedCharacter; // Get the character definition from the player instance
            CharacterUnit unit = Player.Instance.GetCharacterUnit(characterDefinition);
            int level = unit.Level;
            m_Damage = m_AbilityDefinition.GetFinalDotDamagePerTick(level);
            m_Duration = m_AbilityDefinition.GetFinalDotDuration(level);
            // Additional initialization logic if needed
        }
    }
}
