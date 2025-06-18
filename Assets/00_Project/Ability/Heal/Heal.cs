using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class Heal : MonoBehaviour, IAbility
    {
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
        public void Initialize(AbilityDefinition defi, int level)
        {
            // Initialize the ability with the provided definition
            // This can include setting up specific properties or configurations based on the definition
            if (defi != null)
            {
                m_HealAmount = defi.GetFinalHealAmount(level); // Assuming AbilityDefinition has a HealAmount property
            }
        }
    }
}
