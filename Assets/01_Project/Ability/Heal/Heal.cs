using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class Heal : MonoBehaviour, ISelfAbility
    {
        [SerializeField]
        private AbilityDefinition m_AbilityDefinition; // Reference to the ability definition
        [SerializeField]
        private bool m_ApplyOnStart = false; // Flag to apply heal on start
        [SerializeField]
        private int m_HealAmount = 10; // Default heal amount, can be adjusted in the inspector
        [SerializeField]
        private UnityEvent<int> m_OnHealApplied = new(); // Event to notify when healing is applied

        private void Start()
        {
            if (m_ApplyOnStart)
            {
                ApplyHealInternal(m_HealAmount); // Example heal amount, can be adjusted
            }
        }
        public void SetHealAmount(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning("Heal amount must be greater than zero.");
                return;
            }
            m_HealAmount = amount;
        }
        public void ApplyHeal(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning("Heal amount must be greater than zero.");
                return;
            }
            ApplyHealInternal(amount);
        }
        private void ApplyHealInternal(int amount)
        {
            m_OnHealApplied.Invoke(amount);
        }
        public void ApplyHeal()
        {
            ApplyHealInternal(m_HealAmount); // Use the default heal amount
        }
        public void InitializeForPlayer()
        {
            CharacterDefinition characterDefinition = Player.Instance.UsedCharacter; // Get the character definition from the player instance
            CharacterUnit unit = Player.Instance.GetCharacterUnit(characterDefinition);
            int level = unit.Level;
            // Initialize the ability with the provided definition
            // This can include setting up specific properties or configurations based on the definition
            float healRate = m_AbilityDefinition.GetFinalHealRate(level); // Example: get heal rate from ability definition
            int baseAmount = m_AbilityDefinition.GetFinalHealAmount(level); // Example: get heal amount from ability definition
            int characterMaxHealth = unit.FinalStat().Health; // Get the character's maximum health
            m_HealAmount = Mathf.RoundToInt(baseAmount + characterMaxHealth * healRate); // Calculate the final heal amount based on level and rate

            ApplyHealInternal(m_HealAmount); // Apply heal based on the ability definition
        }
    }
}
