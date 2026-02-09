using UnityEngine;

namespace LegionKnight
{
    public class ManaRegen : MonoBehaviour, ISelfAbility
    {
        [SerializeField]
        private AbilityDefinition m_AbilityDefinition;

        public void InitializeForPlayer()
        {
            ApplyAbilityToPlayer();
        }

        private void ApplyAbilityToPlayer()
        {
            Player.Instance.AddManaOvertime(m_AbilityDefinition.ManaRegenStat.ManaRegenFlat, m_AbilityDefinition.ManaRegenStat.RegenDuration);
        }
    }
}
